var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


//Productos 
var inventory = new List<Product>
{
    new() {Id = 0, Name="Mouse", Price=12.4m, Stock=8},
    new() {Id = 1, Name="Keyboard", Price=92.9m, Stock=9},
    new() {Id = 2, Name="Mousepad", Price=81.4m, Stock=0},
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


app.Run();


public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

}







