using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressExpenseController : ControllerBase
    {
        private readonly IAddressExpenseService _addressExpenseService;
        public AddressExpenseController(IAddressExpenseService addressExpenseService)
        {
            _addressExpenseService = addressExpenseService;
        }

        [HttpPost("AddExpenseWithAddress")]
        public async Task<IActionResult> AddAddressExpense(AddressExpenseDto addressExpenseDto)
        {
            return Ok(await _addressExpenseService.AddAddressExpense(addressExpenseDto));
        }

        [HttpGet("GetExpenseWithAddress")]
        public async Task<IActionResult> GetAddressExpenses()
        {
            return Ok(await _addressExpenseService.GetAddressExpenseList());
        }
    }
}