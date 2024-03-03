using System.Data.Entity;
using ProductosAPI.Models;

namespace ProductosAPI.Data
{
    public class ProductosDbContext : DbContext
    {
        public ProductosDbContext() : base("name=ProductosDbContext")
        {
            // Desactivar LazyLoading para evitar referencias circulares en JSON
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuración del nombre único para Producto
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            // Configuración del email único para Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Habilitar Cascading Delete
            modelBuilder.Conventions.Remove<System.Data.Entity.Infrastructure.IncludeMetadataConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}