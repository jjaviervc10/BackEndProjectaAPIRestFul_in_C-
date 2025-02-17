using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Models;

//using ProyectoCapacitacion.Models;
using System.Reflection;

namespace ProyectoCapacitacion.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor que acepta las opciones de configuración (como la cadena de conexión)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<cEmpresa> Empresa { get; set; }
        public DbSet<sUsuario> Usuarios { get; set; }

        public DbSet<cSucursal> Sucursal { get; set; }

        public DbSet<sAcceso> Acceso { get; set; }

        public DbSet<sModulo> Modulo { get; set; }

        public DbSet<sAplicacion> Aplicacion { get; set; }

        public DbSet<sPerfil> Perfil { get; set; }

        public DbSet<rPerfilAcceso> PerfilAccesos { get; set; }


        // Propiedades DbSet para cada entidad (tabla) que queremos manejar


        // Método para configurar las relaciones, si es necesario.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Definir la clave primaria para sUsuario usando Fluent API
            // modelBuilder.Entity<sUsuario>(); // Definir idUsuario como clave primaria

            // Configurar la relación entre cEmpresa y sUsuario
            //modelBuilder.Entity<cEmpresa>()   ;

            // Configuración de la entidad sUsuario
            modelBuilder.Entity<sUsuario>(entity =>
            {
                // Definir la clave primaria
                entity.HasKey(u => u.idUsuario);
            });

            // Configuración de la entidad cEmpresa
            modelBuilder.Entity<cEmpresa>(entity =>
            {
                // Definir la clave primaria
                entity.HasKey(e => e.idEmpresa);

                // Configuración de la relación muchos a uno (cEmpresa -> sUsuario)
                entity.HasOne(e => e.Usuario) // Cada empresa tiene un usuario
                    .WithMany(u => u.Empresas) // Un usuario tiene muchas empresas
                    .HasForeignKey(e => e.idUsuario) // La clave foránea en cEmpresa es idUsuario
                    .OnDelete(DeleteBehavior.Restrict); // Comportamiento en caso de eliminación del usuario
            });

            // Configuracion de la entidad cSucursal
            modelBuilder.Entity<cSucursal>(entity =>
            {
                //Definimos la clave primadria
                entity.HasKey(s => s.idSucursal);

                //Configuración de la relaxión muchos a uno (cSucursal -> cEmpresa)
                entity.HasOne(s => s.Empresa)//Cada sucursal tiene una empresa
                       .WithMany(e => e.Sucursales)//Una empresa tiene muchas sucursales
                       .HasForeignKey(s => s.idEmpresa)//La clave foreanea en cSucursal es idEmpresa
                       .OnDelete(DeleteBehavior.Restrict);

                //Configuracion de  de la relacion muchos a uno (cSucursal -> sUsuario)
                entity.HasOne(s => s.Usuario)//Cda sucursal tiene un usuario
                      .WithMany(u => u.SucursalesU)//Un usuario tiene muchas sucursales
                      .HasForeignKey(s => s.idUsuario)//La FK en cSucursal es idUsuario
                       .OnDelete(DeleteBehavior.Restrict);


                // Validación de que no puede existir más de una sucursal con el mismo nombre en la misma empresa
                entity.HasIndex(s => new { s.nombreSucursal, s.idEmpresa })
                      .IsUnique();
            });

            // Configuracion de la entidad sModulo
            modelBuilder.Entity<sModulo>(entity =>
            {
                //Definimos la clave primaria
                entity.HasKey(m => m.idModulo);

                //Configuracion de la relación muchos a uno
                entity.HasOne(m => m.Aplicacion)//Cada modulo tiene una aplicacion
                       .WithMany(ap => ap.Modulos)//Una aplicacion tine muchos modulos
                       .HasForeignKey(m => m.idAplicacion)//La llave foranea en sModulo es idAplicaicon
                        .OnDelete(DeleteBehavior.Restrict);

            });

            //Configuración de la entidad sAcceso
            modelBuilder.Entity<sAcceso>(entity =>
            {
                //Definimos la clave primaria
                entity.HasKey(a => a.idAcceso);

                //Configuración de la relacion muchos a uno (sAcceso -> sModulo)
                entity.HasOne(a => a.Modulo)//Cada acceso tiene un modulo
                     
                      .WithMany(m => m.Accesos )//Un modulo tiene muchos accesos
                      .HasForeignKey(a => a.idModulo)//La llave foranea en sAcceso es idModulo
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //Configuración de la entidad sAplicaion
            modelBuilder.Entity<sAplicacion>(entity =>
            {
                //Definimos la clave primaria
                entity.HasKey(ap => ap.idAplicacion);

            });

            //Configuracion de la entidad SPerfil
            modelBuilder.Entity<sPerfil>(entity =>
            {
                //Definimos la clave primaria
                entity.HasKey(p => p.idPerfil);



                // Relación muchos a muchos con sAcceso
                entity.HasMany(p => p.Accesos) // Cada perfil puede tener muchos accesos
                      .WithMany(a => a.PerfilesA) // Un acceso puede estar relacionado con muchos perfiles
                      .UsingEntity<rPerfilAcceso>(
                          j => j.HasOne(rpa => rpa.AccesoPerf)  // Relación con sAcceso
                                .WithMany(a => a.RPerfilAccesosA)
                                .HasForeignKey(rpa => rpa.idAcceso),
                          j => j.HasOne(rpa => rpa.PerfilrPerfil)  // Relación con sPerfil
                                .WithMany()
                                .HasForeignKey(rpa => rpa.idPerfil),
                                

                          j =>
                          {
                              j.HasKey(rpa => new { rpa.idAcceso, rpa.idPerfil }); // Clave primaria compuesta
                          }
                      );

                // Relación muchos a uno (sPerfil -> Usuario)
                entity.HasOne(p => p.Usuario) // Cada perfil tiene un usuario
                      .WithMany(u => u.PerfilesU)//Un usuario tiene muchos perfiles
                      .HasForeignKey(p => p.idUsuario)//La llave foranea en sPerfil es idUsuario
                       .OnDelete(DeleteBehavior.Restrict);
            });

            //Configuración de la entidad RPerfilAcceso
            modelBuilder.Entity<rPerfilAcceso>(entity =>
            {
                // Definir la clave primaria compuesta
                entity.HasKey(rpa => new { rpa.idAcceso, rpa.idPerfil });


                //Configuracion de la relación muchos a uno (rPerfilAcceso -> sAcceso)
                entity.HasOne(rpa => rpa.AccesoPerf)//Cada rperfilacceso tiene un acceso
                      .WithMany(a => a.RPerfilAccesosA)//Un rperfiacceso tiene muchos accesos
                      .HasForeignKey(rpa => rpa.idAcceso)//La llave foranea en rperfilacceso es idAcceso
                       .OnDelete(DeleteBehavior.Restrict);

                // Relación muchos a uno (rperfilacceso -> sUsuario)
                entity.HasOne(rpa=> rpa.UsuarioPerf) // Cada perfil tiene un usuario
                      .WithMany(u => u.RPerfilAccesosU)//Un usuario tiene muchos perfiles
                      .HasForeignKey(rpa => rpa.idUsuario)//La llave foranea en reperfilacceso es idUsuario
                       .OnDelete(DeleteBehavior.Restrict);


                //Relacion muchos a uno (rperfilaccceso -> sPerfil)
                entity.HasOne(rpa => rpa.PerfilrPerfil)//Cada rperfilacceso tiee un perfil
                       .WithMany(p => p.RPerfilAccesosP)//Un perfil tiene muchos Rperfilacceso
                       .HasForeignKey(rpa => rpa.idPerfil)//La llave foranea en rperfilacceso es idPerfil
                       .OnDelete(DeleteBehavior.Restrict);
            });

          
        }
    }
}