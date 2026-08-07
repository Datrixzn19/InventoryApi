/*
El * indica a NuGet que descargue el parche más reciente de la rama 9.0 
# (por ejemplo, 9.0.2), asegurando total compatibilidad y seguridad sin saltar a la versión 10.
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.*

# Design es obligatorio para ejecutar las migraciones, convertir el código C# a tablas SQL.
# Debe tener exactamente la misma versión que el paquete base.
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.*

 */

using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // SINTAXIS: DbSet<T> instruye explícitamente al framework para que 
        // mapee la clase 'Product' y genere una tabla física llamada 'Products'.
        // Sin esta línea, la tabla no existirá en SQLite.
        public DbSet<Product> Products { get; set; }
    }
}

/*
 Ejecutar migraciones 
Sirve para que EF cORE traduzca la clase Product en una tabla real dentro de un archivoSQLite
Las migraciones son un sistema de control de versiones para lla bdd. EF Core lee el código C#, detecta los cambios como agregar un nuevo modelo o una nueva propiedad y genera el código necesario para actualizar la estructura de la base de datos sin perder información.



# INSTALAR LA HERRAMIENTA DE LÍNEA DE COMANDOS DE EF
# --global lo instala a nivel del sistema operativo para usarlo en cualquier proyecto.
                dotnet tool install --global dotnet-ef --version 9.0.*



# CREAR LA PRIMERA MIGRACIÓN
# dotnet ef migrations add analiza la clase AppDbContext y los modelos (DbSet).
# Compara el estado actual de tu código con el estado de la base de datos.
# 'InitialCreate' es simplemente el nombre descriptivo que le damos a esta primera captura 
# Esto creará una carpeta llamada Migrations en el proyecto con archivos C# que contienen las instrucciones SQL.
                dotnet ef migrations add InitialCreate



# APLICAR LA MIGRACIÓN A LA BASE DE DATOS
 dotnet ef database update lee los archivos generados en el paso anterior y los ejecuta.
                    dotnet ef database update
 */