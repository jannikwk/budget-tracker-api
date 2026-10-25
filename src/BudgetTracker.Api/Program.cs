using BudgetTracker.Application.Expenses;
using BudgetTracker.Infrastructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Tilføj services til containeren (Dependency Injection)
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddScoped<IExpensesService, ExpenseService>();
builder.Services.AddScoped<IExpensesRepository, ExpensesRepository>();

var app = builder.Build();

// Konfigurer HTTP-anmodningspipelinen (Middleware)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    app.MapGet("/", () => Results.Redirect("/scalar"));
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
