using BuscadorTrabajo.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Aplicacion> Aplicaciones { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Certificacion> Certificaciones { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Desarrollador> Desarrolladores { get; set; }
        public DbSet<Educacion> Educaciones { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Experiencia> Experiencias { get; set; }
        public DbSet<Habilidad> Habilidades { get; set; }
        public DbSet<PerfilProfesional> PerfilesProfesionales { get; set; }
        public DbSet<Perfil_Habilidad> Perfiles_Habilidades { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set; }
        public DbSet<Publicacion_Habilidad> Publicaciones_Habilidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Índices únicos (reglas de negocio) ----------
            modelBuilder.Entity<Empresa>().HasIndex(x => x.NIT).IsUnique();                 // RN02
            modelBuilder.Entity<Cliente>().HasIndex(x => x.Cedula).IsUnique();              // RN03
            modelBuilder.Entity<Desarrollador>().HasIndex(x => x.Cedula).IsUnique();        // RN03
            modelBuilder.Entity<Administrador>().HasIndex(x => x.Cedula).IsUnique();        // RN03
            modelBuilder.Entity<Categoria>().HasIndex(x => x.Nombre).IsUnique();            // RN13
            modelBuilder.Entity<Habilidad>().HasIndex(x => new { x.IdCategoria, x.Nombre }).IsUnique();                     // RN14
            modelBuilder.Entity<Perfil_Habilidad>().HasIndex(x => new { x.IdPerfilProfesional, x.IdHabilidad }).IsUnique();  // RN15
            modelBuilder.Entity<Publicacion_Habilidad>().HasIndex(x => new { x.IdPublicacion, x.IdHabilidad }).IsUnique();   // RN15
            modelBuilder.Entity<Aplicacion>().HasIndex(x => new { x.IdPublicacion, x.IdDesarrollador }).IsUnique();          // RN28

            // ---------- Relaciones ----------
            // Las llaves foráneas se llaman IdXxx, por eso cada relación se declara aquí.
            // Restrict: no deja borrar un registro que otros usan (RN16, RN25, RN32).
            // Cascade: al borrar el padre se borran sus hijos (RN09).

            // Desarrollador 1 - 0..1 PerfilProfesional (RN07)
            modelBuilder.Entity<PerfilProfesional>()
                .HasOne(x => x.Desarrollador).WithOne()
                .HasForeignKey<PerfilProfesional>(x => x.IdDesarrollador)
                .OnDelete(DeleteBehavior.Restrict);

            // PerfilProfesional 1 - N Experiencia, Educacion, Certificacion (RN09)
            modelBuilder.Entity<Experiencia>()
                .HasOne(x => x.PerfilProfesional).WithMany()
                .HasForeignKey(x => x.IdPerfilProfesional)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Educacion>()
                .HasOne(x => x.PerfilProfesional).WithMany()
                .HasForeignKey(x => x.IdPerfilProfesional)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificacion>()
                .HasOne(x => x.PerfilProfesional).WithMany()
                .HasForeignKey(x => x.IdPerfilProfesional)
                .OnDelete(DeleteBehavior.Cascade);

            // Categoria 1 - N Habilidad (RN14)
            modelBuilder.Entity<Habilidad>()
                .HasOne(x => x.Categoria).WithMany()
                .HasForeignKey(x => x.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // PerfilProfesional N - M Habilidad (tabla intermedia Perfil_Habilidad)
            modelBuilder.Entity<Perfil_Habilidad>()
                .HasOne(x => x.PerfilProfesional).WithMany()
                .HasForeignKey(x => x.IdPerfilProfesional)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Perfil_Habilidad>()
                .HasOne(x => x.Habilidad).WithMany()
                .HasForeignKey(x => x.IdHabilidad)
                .OnDelete(DeleteBehavior.Restrict);

            // Empresa / Cliente / Categoria 1 - N Publicacion (RN19, RN17)
            modelBuilder.Entity<Publicacion>()
                .HasOne(x => x.Empresa).WithMany()
                .HasForeignKey(x => x.IdEmpresa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Publicacion>()
                .HasOne(x => x.Cliente).WithMany()
                .HasForeignKey(x => x.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Publicacion>()
                .HasOne(x => x.Categoria).WithMany()
                .HasForeignKey(x => x.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            // Publicacion N - M Habilidad (tabla intermedia Publicacion_Habilidad)
            modelBuilder.Entity<Publicacion_Habilidad>()
                .HasOne(x => x.Publicacion).WithMany()
                .HasForeignKey(x => x.IdPublicacion)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Publicacion_Habilidad>()
                .HasOne(x => x.Habilidad).WithMany()
                .HasForeignKey(x => x.IdHabilidad)
                .OnDelete(DeleteBehavior.Restrict);

            // Publicacion / Desarrollador 1 - N Aplicacion (RN25, RN27)
            modelBuilder.Entity<Aplicacion>()
                .HasOne(x => x.Publicacion).WithMany()
                .HasForeignKey(x => x.IdPublicacion)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Aplicacion>()
                .HasOne(x => x.Desarrollador).WithMany()
                .HasForeignKey(x => x.IdDesarrollador)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Valores de dinero: 18 dígitos, 2 decimales ----------
            modelBuilder.Entity<Publicacion>().Property(x => x.Salario).HasPrecision(18, 2);
            modelBuilder.Entity<Publicacion>().Property(x => x.Presupuesto).HasPrecision(18, 2);
            modelBuilder.Entity<Aplicacion>().Property(x => x.MontoPropuesto).HasPrecision(18, 2);
        }
    }
}
