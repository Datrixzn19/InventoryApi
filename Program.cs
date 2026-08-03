var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


//Productos 
var inventory = new List<Product>
{
    new() {Name="Mouse", Price=12.4m, Stock=8},
    new() {Name="Keyboard", Price=92.9m, Stock=9},
    new() {Name="Mousepad", Price=81.4m, Stock=0},
};


    //GET
//Obtener todos los productos
app.MapGet("/api/products/", () => inventory);



app.Run();


public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

}







