using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


//Productos 
var inventory = new List<Product>
{
    new() {Id = 1, Name="Mouse", Price=12.4m, Stock=8},
    new() {Id = 2, Name="Keyboard", Price=92.9m, Stock=9},
    new() {Id = 3, Name="Mousepad", Price=81.4m, Stock=0},
};


    //GET
//Obtener todos los productos
app.MapGet("/api/products/", () => inventory);
//Obtener un producto por id 
app.MapGet("/api/products/{id:int}", (int id) => 
{
    //Seleccionarlo
    var product = inventory.FirstOrDefault(p => p.Id == id);
    //Comprobar que exista
    return product is null ? Results.NotFound() : Results.Ok(product);

});
    
    // POST
app.MapPost("/api/products/lote", (List<Product> newProducts) =>
{
    //Verificar que no venga vacia 
    if (newProducts.Count() == 0) return Results.BadRequest();
    //Contabilizar los IDs
    int nextID = inventory.Count() > 0 ? inventory.Max(p => p.Id) + 1 : 1;
    //Asignar los IDs
    foreach (var product in newProducts)
    {
        product.Id = nextID;//asignamos el id
        nextID++;//Aumentamos para el siguiente producto
    }
    //Add Range agrega los productos de golpe 
    inventory.AddRange(newProducts);
    return Results.Ok(newProducts);
});



//PUT
app.MapPut("api/products/{id:int}", (Product productUpdated, int id) =>
{
    //Verificamos que no venga vacia
    if(productUpdated is null) return Results.BadRequest();
    //Verificamos que el producto si exista
    var product = inventory.FirstOrDefault(p => p.Id == id);
    if (product is null) return Results.NotFound();
    //Actualizamos los campos
    product.Name = productUpdated.Name;
    product.Price = productUpdated.Price;
    product.Stock = productUpdated.Stock;

    return Results.NoContent();

});

//DELETE
app.MapDelete("api/products/{id:int}", (int id) =>
{
    //Verificamos que el producto exista
    var product = inventory.FirstOrDefault(p => p.Id == id );
    if (product is null) return Results.NotFound();
    //Eliminamos 
    inventory.Remove(product);
    return Results.NoContent();

});

//Cabeceras
app.MapGet("api/products/safe", ([FromHeader(Name = "Authorization")] string? authHeader ) =>
{
    if (string.IsNullOrWhiteSpace(authHeader)) return Results.Unauthorized(); //err 401

    if (authHeader.StartsWith("Bearer ") && authHeader == "Bearer MiTokenSecreto123")
    { 
    return Results.Ok(new { Mensaje = "Usuario Autorizado" });
    }

    return Results.Unauthorized();
});



app.Run();


public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

}







