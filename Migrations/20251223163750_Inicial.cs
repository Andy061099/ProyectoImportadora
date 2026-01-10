using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportadoraApi.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Almacenes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    nombreencargado = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacenes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Contenedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    FechaArribo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CostoNacionalizacion = table.Column<decimal>(type: "numeric", nullable: false),
                    CostoTransporte = table.Column<decimal>(type: "numeric", nullable: false),
                    CostoDescarga = table.Column<decimal>(type: "numeric", nullable: false),
                    PorcentajeVendido = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    UnidadMedida = table.Column<string>(type: "text", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioMayorista = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioMinorista = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Rol = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventarios_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContenedorDetalle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContenedorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AlmacenDestinoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "numeric", nullable: false),
                    Packa = table.Column<bool>(type: "boolean", nullable: false),
                    Cantproductosxpacka = table.Column<decimal>(type: "numeric", nullable: false),
                    Cantidadmerma = table.Column<decimal>(type: "numeric", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContenedorDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContenedorDetalle_Almacenes_AlmacenDestinoId",
                        column: x => x.AlmacenDestinoId,
                        principalTable: "Almacenes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContenedorDetalle_Contenedores_ContenedorId",
                        column: x => x.ContenedorId,
                        principalTable: "Contenedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContenedorDetalle_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CierresDiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalIngresos = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalGastos = table.Column<decimal>(type: "numeric", nullable: false),
                    GananciaNeta = table.Column<decimal>(type: "numeric", nullable: false),
                    CantidadVentas = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresDiarios_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_CierresDiarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosFinancieros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: true),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    ReferenciaTipo = table.Column<string>(type: "text", nullable: false),
                    ReferenciaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosFinancieros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosFinancieros_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_RegistrosFinancieros_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoVenta = table.Column<int>(type: "integer", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cliente = table.Column<string>(type: "text", nullable: true),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    consignacionid = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventarioProductos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockActual = table.Column<decimal>(type: "numeric", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "numeric", nullable: false),
                    UbicacionFisica = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Packa = table.Column<bool>(type: "boolean", nullable: false),
                    Cantproductosxpacka = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioProductos_Inventarios_InventarioId",
                        column: x => x.InventarioId,
                        principalTable: "Inventarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CierreDiarioRegistros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CierreDiarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistroFinancieroId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierreDiarioRegistros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierreDiarioRegistros_CierresDiarios_CierreDiarioId",
                        column: x => x.CierreDiarioId,
                        principalTable: "CierresDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CierreDiarioRegistros_RegistrosFinancieros_RegistroFinancie~",
                        column: x => x.RegistroFinancieroId,
                        principalTable: "RegistrosFinancieros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consignaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    MontoPendiente = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consignaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consignaciones_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventarioProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoMovimiento = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    StockAnterior = table.Column<decimal>(type: "numeric", nullable: false),
                    StockPosterior = table.Column<decimal>(type: "numeric", nullable: false),
                    OrSigen = table.Column<int>(type: "integer", nullable: false),
                    ReferenciaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_InventarioProductos_InventarioProduct~",
                        column: x => x.InventarioProductoId,
                        principalTable: "InventarioProductos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VentaDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VentaId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventarioProductoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_InventarioProductos_InventarioProductoId",
                        column: x => x.InventarioProductoId,
                        principalTable: "InventarioProductos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VentaDetalles_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsignacionMovimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsignacionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsignacionMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsignacionMovimientos_Consignaciones_ConsignacionId",
                        column: x => x.ConsignacionId,
                        principalTable: "Consignaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsignacionMovimientos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioRegistros_CierreDiarioId",
                table: "CierreDiarioRegistros",
                column: "CierreDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CierreDiarioRegistros_RegistroFinancieroId",
                table: "CierreDiarioRegistros",
                column: "RegistroFinancieroId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresDiarios_AlmacenId",
                table: "CierresDiarios",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_CierresDiarios_UsuarioId",
                table: "CierresDiarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Consignaciones_VentaId",
                table: "Consignaciones",
                column: "VentaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsignacionMovimientos_ConsignacionId",
                table: "ConsignacionMovimientos",
                column: "ConsignacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsignacionMovimientos_UsuarioId",
                table: "ConsignacionMovimientos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ContenedorDetalle_AlmacenDestinoId",
                table: "ContenedorDetalle",
                column: "AlmacenDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContenedorDetalle_ContenedorId",
                table: "ContenedorDetalle",
                column: "ContenedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContenedorDetalle_ProductoId",
                table: "ContenedorDetalle",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioProductos_InventarioId",
                table: "InventarioProductos",
                column: "InventarioId");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioProductos_ProductoId",
                table: "InventarioProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_AlmacenId",
                table: "Inventarios",
                column: "AlmacenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_InventarioProductoId",
                table: "MovimientosInventario",
                column: "InventarioProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_UsuarioId",
                table: "MovimientosInventario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFinancieros_AlmacenId",
                table: "RegistrosFinancieros",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFinancieros_UsuarioId",
                table: "RegistrosFinancieros",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_InventarioProductoId",
                table: "VentaDetalles",
                column: "InventarioProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaDetalles_VentaId",
                table: "VentaDetalles",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_AlmacenId",
                table: "Ventas",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UsuarioId",
                table: "Ventas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CierreDiarioRegistros");

            migrationBuilder.DropTable(
                name: "ConsignacionMovimientos");

            migrationBuilder.DropTable(
                name: "ContenedorDetalle");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "VentaDetalles");

            migrationBuilder.DropTable(
                name: "CierresDiarios");

            migrationBuilder.DropTable(
                name: "RegistrosFinancieros");

            migrationBuilder.DropTable(
                name: "Consignaciones");

            migrationBuilder.DropTable(
                name: "Contenedores");

            migrationBuilder.DropTable(
                name: "InventarioProductos");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Inventarios");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Almacenes");
        }
    }
}
