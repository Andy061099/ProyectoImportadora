using ImportadoraApi.Models;
using ImportadoraApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace ImportadoraApi.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // =========================
            // USUARIOS
            // =========================
            if (!await context.Usuarios.AnyAsync())
            {
                context.Usuarios.Add(new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Admin",
                    Email = "admin@importadora.com",
                    PasswordHash = PasswordHelper.Hash("admin"),
                    Rol = "Admin",
                    Activo = true
                });
            }

            // =========================
            // ALMACEN + INVENTARIO
            // =========================
            Almacen almacenPrincipal;

            if (!await context.Almacenes.AnyAsync())
            {
                almacenPrincipal = new Almacen
                {
                    id = Guid.NewGuid(),
                    nombre = "Almacén Principal",
                    nombreencargado = "Encargado General",
                    Ubicacion = "Central",
                    Descripcion = "Almacén principal"
                };

                var inventario = new Inventario
                {
                    Id = Guid.NewGuid(),
                    AlmacenId = almacenPrincipal.id,
                    FechaCreacion = DateTime.UtcNow
                };

                almacenPrincipal.inventario = inventario;

                context.Almacenes.Add(almacenPrincipal);
            }
            else
            {
                almacenPrincipal = await context.Almacenes
                    .Include(a => a.inventario)
                    .FirstAsync();
            }

            // =========================
            // PRODUCTOS
            // =========================
            if (!await context.Productos.AnyAsync())
            {
                var arroz = new Producto
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Arroz",
                    Codigo = "ARZ-001",
                    Descripcion = "Arroz blanco premium",
                    UnidadMedida = "kg",
                    CostoUnitario = 10,
                    PrecioMayorista = 12,
                    PrecioMinorista = 15,
                    MonedaDeEntrada = TipoMoneda.Dolar
                };

                var leche = new Producto
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Leche",
                    Codigo = "LCH-001",
                    Descripcion = "Leche entera 1L",
                    UnidadMedida = "litro",
                    CostoUnitario = 5,
                    PrecioMayorista = 7,
                    PrecioMinorista = 9,
                    MonedaDeEntrada = TipoMoneda.Dolar
                };

                context.Productos.AddRange(arroz, leche);

                // =========================
                // INVENTARIO PRODUCTOS
                // =========================
                if (almacenPrincipal?.inventario != null)
                {
                    context.InventarioProductos.AddRange(
                        new InventarioProducto
                        {
                            Id = Guid.NewGuid(),
                            InventarioId = almacenPrincipal.inventario.Id,
                            ProductoId = arroz.Id,
                            StockActual = 100
                        },
                        new InventarioProducto
                        {
                            Id = Guid.NewGuid(),
                            InventarioId = almacenPrincipal.inventario.Id,
                            ProductoId = leche.Id,
                            StockActual = 50
                        }
                    );
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
