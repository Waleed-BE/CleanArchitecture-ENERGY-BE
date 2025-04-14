using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Features.Business.ExpenseCategories.Commands;
using CleanArchitecture.Application.Features.Business.ExpenseCategories.Queries;
using CleanArchitecture.Application.Features.Business.ExpenseCategory.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ExpenseCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Get-All-Expense-Categories")]
        public async Task<ActionResult<List<ExpenseCategoryDTO>>> GetAllExpenseCategories([FromQuery] bool activeOnly = false)
        {
            var query = new GetExpenseCategoryListQuery { ActiveOnly = activeOnly };
            var expenseCategory = await _mediator.Send(query);
            return Ok(expenseCategory);
        }

        [HttpPost("Create-Expense-Category")]
        public async Task<ActionResult<Guid>> CreateExpenseType([FromBody] CreateExpenseTypeDto createExpenseTypeDto)
        {
            var command = new CreateExpenseCategoryCommand
            {
                ExpenseCategoryName = createExpenseTypeDto.ExpenseName,
                Description = createExpenseTypeDto.Description
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetExpenseCategoryById), new { id = result }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseTypeDto>> GetExpenseCategoryById(Guid id)
        {
            // This would require implementing a GetExpenseTypeByIdQuery
            // For brevity, I'm leaving this as a placeholder
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExpenseCategory([FromBody] UpdateExpenseCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
