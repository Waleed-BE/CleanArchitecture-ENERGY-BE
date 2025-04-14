using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.DTOs.StripePayment;
using CleanArchitecture.Application.Features.Business.ExpenseTypes.Commands;
using CleanArchitecture.Application.Features.Business.StripePurchasePlan.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace CleanArchitecture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public StripeController(IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }
        
        [HttpGet("get-all-products")]
        public IActionResult GetAllProducts()
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:APIKey"];

            var productService = new ProductService();
            var products = productService.List(new ProductListOptions
            {
                Limit = 100 // Adjust limit as per your needs
            });

            var priceService = new PriceService();

            var response = products.Select(product =>
            {
                var prices = priceService.List(new PriceListOptions
                {
                    Product = product.Id,
                    Limit = 100
                });

                var priceList = prices.Select(price => new
                {
                    price.Id,
                    Amount = price.UnitAmount.HasValue ? Math.Round(price.UnitAmount.Value / 100m, 4) : 0,
                    price.Currency,
                    Interval = price.Recurring?.Interval ?? "one_time",
                    price.Type
                });

                return new
                {
                    product.Id,
                    product.Name,
                    product.Description,
                    Prices = priceList
                };
            });
            return Ok(response);
        }

        [HttpPost("purchase-plan")]
        public async Task<IActionResult> PurchasePlan(PurchasePlanDto purchasePlanDto)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:APIKey"];

            var priceService = new PriceService();
            var price = priceService.Get(purchasePlanDto.StripePriceId);

            if (price == null)
            {
                return BadRequest("Invalid price ID.");
            }

            // Dynamically determine mode based on price type
            var mode = price.Type == "recurring" ? "subscription" : "payment";

            var options = new SessionCreateOptions
            {
                Customer = purchasePlanDto.StripeCustomerId,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = purchasePlanDto.StripePriceId,
                        Quantity = 1
                    }
                },
                Mode = mode,
                SuccessUrl = _configuration["FrontendAppUrl"] + "/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = _configuration["FrontendAppUrl"] + "/cancel",
                Metadata = new Dictionary<string, string>
                {
                    { "UserId", purchasePlanDto.UserId.ToString() },
                    { "PlanName", purchasePlanDto.PlanName },
                    { "PlanPurchaseType", purchasePlanDto.PlanPurchaseType }
                }
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new
            {
                sessionId = session.Id,
                url = session.Url
            });
        }


        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment(string sessionId, PurchasePlanDto purchasePlanDto)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:APIKey"];

            var service = new SessionService();
            var session = service.Get(sessionId);

            if (session.PaymentStatus == "paid")
            {
                // Update user plan in database etc.
                // Save purchase info to DB
                var command = new CreateStripePurchaseCommand
                {
                    UserId = purchasePlanDto.UserId.ToString(),
                    StripePriceId = purchasePlanDto.StripePriceId,
                    PlanName = purchasePlanDto.PlanName,
                    Price = purchasePlanDto.Price,
                    Currency = purchasePlanDto.Currency,
                    Interval = purchasePlanDto.Interval,
                    PlanPurchaseType = purchasePlanDto.PlanPurchaseType
                };

                var result = await _mediator.Send(command);
                
                return Ok(new { success = true, message = "Payment successful", Token = result });
            }

            return BadRequest(new { success = false, message = "Payment not completed" });
        }
    }
}