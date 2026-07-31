var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();


app.MapGet("/api/test", () =>
{
    return Results.Ok(new { Mensaje = "Test" });
});

app.Run();