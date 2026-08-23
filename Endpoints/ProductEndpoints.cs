//Un metodo de extension permite agregar funcionalidades a clases exitentes que no he creado ej WebAplication sin modificar su codigo original 
using InventoryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using System.Threading.Tasks;

namespace InventoryApi.Endpoints
{
    //Static es obligatorio para metodos de extension 
    public static class ProductEndpoints
    {
            
             
        //Productos 
        //static indica que la lista pertenece a la clase en si, no a una instancia 
        private static readonly List<Product> _inventory = new()
        {
            new() {Id = 1, Name="Mouse", Price=12.4m, Stock=8},
            new() {Id = 2, Name="Keyboard", Price=92.9m, Stock=9},
            new() {Id = 3, Name="Mousepad", Price=81.4m, Stock=0},
        };

        //la palabra this es lo que lo convierte en un metodo de extension 
        //si la clase es static los metodos tambien deben serlo 
        public static async Task MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/products"); //Prefijo para las demas 
            /*
            // Probar conexion a efcore
            app.MapGet("/api/products/debug", (AppDbContext db) =>
            {
                var debugInfo = new
                {
                    CanConnect = db.Database.CanConnect(),
                    ConnectionString = db.Database.GetConnectionString(),
                    Provider = db.Database.ProviderName
                };
                return Results.Ok(debugInfo);
            });

             */
            //GET
            //Obtener todos los productos
            group.MapGet("/", async (AppDbContext db) =>
            {
                /*
                 * Con sql
                // FromSqlRaw ejecuta una consulta SQL nativa directa a tu base de datos SQLite.
                var products = await db.Products
                                       .FromSqlRaw($"SELECT * FROM Products")
                                       .ToListAsync();//toma los resultados de ese SQL y los convierte de forma asíncrona en una lista 

                return Results.Ok(products);
                */
                var products = await db.Products
                    .AsNoTracking()//Esta linea va solo si se va a leer los datos, si vamos a mod quitamos
                    .ToListAsync();
                return Results.Ok(products);
            });

            //Obtener un solo elemento
            group.MapGet("/{id:int}", async (AppDbContext db, int id ) =>
            {
                var product = await db.Products.FirstOrDefaultAsync(u => u.Id == id);

                return product is null ? Results.NotFound() : Results.Ok(product);
            });
            
            //POST
            //agregar un solo producto
            group.MapPost("/", async (Product newProduct, AppDbContext db) => {
                db.Products.Add(newProduct);//toma esto y lo pone en memoria
                await db.SaveChangesAsync();//traduce los cambios en memoria a consultas sql y modifica la bdd 
                //EfCore pone automaticamente los ids incrementales
                return Results.Created($"/api/products/{newProduct.Id}", newProduct);//El estandar pide que demos la ruta donde se ha creado, opcionalmente el elemento en si
            });
            //agregar una lista de productos 
            group.MapPost("/lote", async (List<Product> newProducts, AppDbContext db) => {
                if (newProducts.Count == 0) return Results.BadRequest("La no puede estar vacia");
                db.Products.AddRange(newProducts);//Add range es para agregar mas de un producto 
                await db.SaveChangesAsync();
                return Results.Created();
            });


            //PUT
            group.MapPut("/{id:int}", async (Product updatedProduct, int id, AppDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);//Metodo especifico para buscar IDs
                if (product is null) return Results.NotFound();//verificamos que si haya ese producto
                //Actualizamos los campos
                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Stock = updatedProduct.Stock;
                await db.SaveChangesAsync();             
                return Results.NoContent();

            });

            //DELETE 
            group.MapDelete("/{id:int}",async (int id, AppDbContext db) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product is null) return Results.NotFound();

                db.Products.Remove(product);
                await db.SaveChangesAsync();
                return Results.Ok(product);
            });




        }

    }
}


/*
        Antes de implementar EFCore


            
        //Productos 
        //static indica que la lista pertenece a la clase en si, no a una instancia 
        private static readonly List<Product> _inventory = new()
        {
            new() {Id = 1, Name="Mouse", Price=12.4m, Stock=8},
            new() {Id = 2, Name="Keyboard", Price=92.9m, Stock=9},
            new() {Id = 3, Name="Mousepad", Price=81.4m, Stock=0},
        };
    
        //la palabra this es lo que lo convierte en un metodo de extension 
        //si la clase es static los metodos tambien deben serlo 
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/products"); //Prefijo para las demas 

            //GET
            //Obtener todos los productos
            app.MapGet("/", () => _inventory);
            //Obtener un producto por id 
            app.MapGet("/{id:int}", (int id) =>
            {
                //Seleccionarlo
                var product = _inventory.FirstOrDefault(p => p.Id == id);
                //Comprobar que exista
                return product is null ? Results.NotFound() : Results.Ok(product);

            });

            // POST
            app.MapPost("/lote", (List<Product> newProducts) =>
            {
                //Verificar que no venga vacia 
                if (newProducts.Count() == 0) return Results.BadRequest();
                //Contabilizar los IDs
                int nextID = _inventory.Count() > 0 ? _inventory.Max(p => p.Id) + 1 : 1;
                //Asignar los IDs
                foreach (var product in newProducts)
                {
                    product.Id = nextID;//asignamos el id
                    nextID++;//Aumentamos para el siguiente producto
                }
                //Add Range agrega los productos de golpe 
                _inventory.AddRange(newProducts);
                return Results.Ok(newProducts);
            });



            //PUT
            app.MapPut("/{id:int}", (Product productUpdated, int id) =>
            {
                //Verificamos que no venga vacia
                if (productUpdated is null) return Results.BadRequest();
                //Verificamos que el producto si exista
                var product = _inventory.FirstOrDefault(p => p.Id == id);
                if (product is null) return Results.NotFound();
                //Actualizamos los campos
                product.Name = productUpdated.Name;
                product.Price = productUpdated.Price;
                product.Stock = productUpdated.Stock;

                return Results.NoContent();

            });

            //DELETE
            app.MapDelete("/{id:int}", (int id) =>
            {
                //Verificamos que el producto exista
                var product = _inventory.FirstOrDefault(p => p.Id == id);
                if (product is null) return Results.NotFound();
                //Eliminamos 
                _inventory.Remove(product);
                return Results.NoContent();

            });

            //Cabeceras
            app.MapGet("/safe", ([FromHeader(Name = "Authorization")] string? authHeader) =>
            {
                if (string.IsNullOrWhiteSpace(authHeader)) return Results.Unauthorized(); //err 401

                if (authHeader.StartsWith("Bearer ") && authHeader == "Bearer MiTokenSecreto123")
                {
                    return Results.Ok(new { Mensaje = "Usuario Autorizado" });
                }

                return Results.Unauthorized();
            });
 */