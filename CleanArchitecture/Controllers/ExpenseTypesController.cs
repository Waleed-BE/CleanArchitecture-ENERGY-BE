using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Features.Business.ExpenseTypes.Commands;
using CleanArchitecture.Application.Features.Business.ExpenseTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpenseTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("Get-All-Expense-Types")]
        public async Task<ActionResult<List<ExpenseTypeDto>>> GetAllExpenseTypes([FromQuery] bool activeOnly = false)
        {
            var query = new GetExpenseTypeListQuery { ActiveOnly = activeOnly };
            var expenseTypes = await _mediator.Send(query);
            return Ok(expenseTypes);
        }

        [HttpPost("Add-Expense-Type")]
        public async Task<ActionResult<Guid>> CreateExpenseType([FromBody] CreateExpenseTypeDto createExpenseTypeDto)
        {
            var command = new CreateExpenseTypeCommand
            {
                ExpenseName = createExpenseTypeDto.ExpenseName,
                Description = createExpenseTypeDto.Description,
                ExpenseCategoryId = createExpenseTypeDto.ExpenseCategoryId,
                UnitOfMeasurement = createExpenseTypeDto.UnitOfMeasurement,
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetExpenseTypeById), new { id = result }, result);
        }

        [HttpPut("Update-Expense-Type")]
        public async Task<IActionResult> UpdateExpenseType([FromBody] UpdateExpenseTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);    
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseTypeDto>> GetExpenseTypeById(Guid id)
        {
            // This would require implementing a GetExpenseTypeByIdQuery
            // For brevity, I'm leaving this as a placeholder
            throw new NotImplementedException();
        }

        // Additional actions for update, delete, etc. would go here
    }
}