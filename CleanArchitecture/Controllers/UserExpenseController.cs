using CleanArchitecture.Application.Features.Business.UserExpenses.Commands;
using CleanArchitecture.Application.Features.Business.UserExpense.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/user-expenses")]
    [ApiController]
    public class UserExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IGeminiService _geminiService;

        public UserExpenseController(IMediator mediator, IGeminiService geminiService)
        {
            _mediator = mediator;
            _geminiService = geminiService;
        }

        [HttpPost("Create-User-Expense")]
        public async Task<IActionResult> CreateUserExpense([FromBody] CreateUserExpenseCommand command)
        {
            var expenseId = await _mediator.Send(command);
            return Ok(new { ExpenseId = expenseId, Message = "Expense added successfully." });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserExpenses(string userId)
        {
            var expenses = await _mediator.Send(new GetUserExpensesQuery { UserId = userId });
            return Ok(expenses);
        }

        [HttpPut("Update-User-Expense")]
        public async Task<IActionResult> updateUserExpense([FromBody] UpdateUserExpenseCommand updateUserExpenseCommand)
        {
            var result = await _mediator.Send(updateUserExpenseCommand);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get-AI-Response")]
        public async Task<IActionResult> getAIResponse(Guid UserId)
        {
            var textPrompt = "Please analyze the following expense data based on dates and provide valuable insights focused on cost-saving opportunities. Return the output in clean, structured HTML using Tailwind CSS classes.\r\nUse visually appealing components such as:\r\nCards (div with padding, rounded corners, shadows) Headings (h2, h3 with text-xl, font-semibold) Lists (ul, li with list-disc, ml-4) Tables (with table-auto, border, text-left, etc.) Group insights into clear sections (e.g., Key Insights, Cost Breakdown, Recommendations), and ensure the layout is readable and modern like a professional web dashboard. do not provide the key improvments in the design and markup just keep it data oriented do not include Key improvements and explanations anywhere in the markup even outside the html tag and return the data only that is inside the html tag. in the end of all the tables and summary give the detailed recommendations of cost effectiveness. This prompt should be recommendations and suggestions oriented for the best ways to save and areas of drainage.";
            return Ok(await _geminiService.GenerateContentAsync(UserId, textPrompt));
        }
    }
}
