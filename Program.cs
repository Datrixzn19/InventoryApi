using InventoryApi.Data;
using InventoryApi.Endpoints;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Extraemos la cadena a una variable para implementar el patrón Fail-Fast
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");
}

// nyectamos el contexto de base de datos usando la cadena ya validada.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// using crea un entorno (scope) temporal aislado.
// Al terminar el bloque, la memoria utilizada para pedir la base de datos se libera.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Migrate() revisa el archivo físico inventory.db.
    // Si le faltan tablas generadas en tus Migraciones las crea automáticamente sin destruir los datos que ya existen.
    db.Database.Migrate();
}


// enlaza todas tus rutas bajo el prefijo "/api/products" al ciclo de vida del servidor.
app.MapProductEndpoints();


app.Run();