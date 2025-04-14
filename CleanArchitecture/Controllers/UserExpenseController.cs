using CleanArchitecture.Application.Features.Business.UserExpenses.Commands;
using CleanArchitecture.Application.Features.Business.UserExpense.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.API.Controllers
{
    [Route("api/user-expenses")]
    [ApiController]
    public class UserExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserExpenseController(IMediator mediator)
        {
            _mediator = mediator;
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
    }
}
