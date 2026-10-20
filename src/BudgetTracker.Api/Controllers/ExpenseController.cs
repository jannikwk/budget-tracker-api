using BudgetTracker.Application.Expenses;
using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpensesService _expensesService;

    public ExpensesController(IExpensesService expensesService)
    {
        _expensesService = expensesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var expenses = await _expensesService.GetAllAsync(cancellationToken);
        return Ok(expenses);
    }
}

