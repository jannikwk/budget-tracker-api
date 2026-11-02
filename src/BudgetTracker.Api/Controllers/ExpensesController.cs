using BudgetTracker.Application.Expenses;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(IExpensesService expensesService) : ControllerBase
{
    private readonly IExpensesService _expensesService = expensesService;

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create(CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var expense = await _expensesService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var expenses = await _expensesService.GetAllAsync(cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        var expense = await _expensesService.GetByIdAsync(id, cancellationToken);
        
        if (expense == null) {
            return NotFound();
        }

        return Ok(expense);
    }
}

