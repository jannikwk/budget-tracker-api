var builder = WebApplication.CreateBuilder(args);

// Tilføj services til containeren (Dependency Injection)
builder.Services.AddOpenApi();

var app = builder.Build();

// Konfigurer HTTP-anmodningspipelinen (Middleware)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
