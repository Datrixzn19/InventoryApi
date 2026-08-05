//Un metodo de extension permite agregar funcionalidades a clases exitentes que no he creado ej WebAplication sin modificar su codigo original 
using InventoryApi.Models;
using Microsoft.AspNetCore.Mvc;

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

        }
    }
}
