using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using System.Security.Cryptography.X509Certificates;
public class AppDbContext : DbContext
{
    public DbSet<CierreDiario> CierresDiarios { get; set; } = null!;
    public DbSet<CierreDiarioRegistro> CierreDiarioRegistros { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Almacen> Almacenes { get; set; } = null!;
    public DbSet<Inventario> Inventarios { get; set; } = null!;
    public DbSet<InventarioProducto> InventarioProductos { get; set; } = null!;
    public DbSet<Contenedor> Contenedores { get; set; } = null!;
    public DbSet<ContenedorDetalle> ContenedorDetalles { get; set; } = null!;
    public DbSet<Producto> Productos { get; set; } = null!;
    public DbSet<MovimientoInventario> MovimientosInventario { get; set; } = null!;

    public DbSet<Merma> Mermas { get; set; } = null!;
    public DbSet<Venta> Ventas { get; set; } = null!;
    public DbSet<VentaDetalle> VentaDetalles { get; set; } = null!;
    public DbSet<Consignacion> Consignaciones { get; set; } = null!;
    public DbSet<ConsignacionMovimiento> ConsignacionMovimientos { get; set; } = null!;
    public DbSet<RegistroFinanciero> RegistrosFinancieros { get; set; } = null!;

    public DbSet<DistribucionProducto> DistribucionProductos { get; set; } = null!;
    public DbSet<CostosContenedor> CostosContenedores { get; set; } = null!;


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar claves primarias si quieres explícito
        modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
        modelBuilder.Entity<Almacen>().HasKey(a => a.id);
        modelBuilder.Entity<Inventario>().HasKey(i => i.Id);
        modelBuilder.Entity<InventarioProducto>().HasKey(ip => ip.Id);
        modelBuilder.Entity<Contenedor>().HasKey(c => c.Id);
        modelBuilder.Entity<MovimientoInventario>().HasKey(m => m.Id);
        modelBuilder.Entity<Venta>().HasKey(v => v.Id);
        modelBuilder.Entity<VentaDetalle>().HasKey(vd => vd.Id);
        modelBuilder.Entity<Consignacion>().HasKey(c => c.Id);
        modelBuilder.Entity<ConsignacionMovimiento>().HasKey(cm => cm.Id);
        modelBuilder.Entity<RegistroFinanciero>().HasKey(rf => rf.Id);

        // Relaciones

        // Inventario - InventarioProducto
        modelBuilder.Entity<Inventario>()
            .HasMany(i => i.Productos)
            .WithOne(ip => ip.Inventario)
            .HasForeignKey(ip => ip.InventarioId).OnDelete(DeleteBehavior.Cascade);

        // Almacen - Inventario
        // Relación 1 a 1 Almacen ↔ Inventario
        modelBuilder.Entity<Almacen>()
            .HasOne(a => a.inventario)
            .WithOne(i => i.Almacen)
            .HasForeignKey<Inventario>(i => i.AlmacenId).OnDelete(DeleteBehavior.Cascade);


        // Venta - VentaDetalle
        modelBuilder.Entity<Venta>()
            .HasMany(v => v.Detalles)
            .WithOne(d => d.Venta)
            .HasForeignKey(d => d.VentaId).OnDelete(DeleteBehavior.Cascade);

        // Consignacion - ConsignacionMovimientos
        modelBuilder.Entity<Consignacion>()
            .HasMany(c => c.Movimientos)
            .WithOne(cm => cm.Consignacion)
            .HasForeignKey(cm => cm.ConsignacionId).OnDelete(DeleteBehavior.Cascade);

        // Usuario - movimientos, ventas, registros financieros
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Movimientos)
            .WithOne(m => m.Usuario)
            .HasForeignKey(m => m.UsuarioId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Ventas)
            .WithOne(v => v.Usuario)
            .HasForeignKey(v => v.UsuarioId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.RegistrosFinancieros)
            .WithOne(r => r.Usuario)
            .HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Consignaciones)
            .WithOne(cm => cm.Usuario)
            .HasForeignKey(cm => cm.UsuarioId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venta>()
        .HasOne(V => V.Consignacion)
        .WithOne(C => C.Venta)
        .HasForeignKey<Consignacion>(C => C.VentaId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contenedor>()
        .HasMany(C => C.Detalles)
        .WithOne(D => D.Contenedor)
        .HasForeignKey(D => D.ContenedorId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Producto>()
        .HasMany(P => P.Inventarios)
        .WithOne(D => D.Producto)
        .HasForeignKey(D => D.ProductoId).OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Contenedor>()
        .HasMany(p => p.Costos)
        .WithOne(p => p.contenedorasignado)
        .HasForeignKey(d => d.Idcotenedor).OnDelete(DeleteBehavior.Cascade);



        modelBuilder.Entity<CierreDiarioRegistro>()
    .HasOne(x => x.CierreDiario)
        .WithMany(c => c.Registros)
        .HasForeignKey(x => x.CierreDiarioId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventarioProducto>()
        .HasMany(IP => IP.Movimientos)
        .WithOne(M => M.InventarioProducto)
        .HasForeignKey(M => M.InventarioProductoId).OnDelete(DeleteBehavior.Cascade);




    }
}
