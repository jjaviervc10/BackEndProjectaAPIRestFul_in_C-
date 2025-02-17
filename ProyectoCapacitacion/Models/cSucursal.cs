using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProyectoCapacitacion.Models
{
    [Table("cSucursal")]
    public class cSucursal
    {
        [Key]
        [Column("idSucursal")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Autoincremental
        public int idSucursal { get; set; }//PK

        public string nombreSucursal { get; set; }
        public string? ciudad { get; set; }
        public string? estado { get; set; }

        public Boolean? activo { get; set; }  
        
        public DateTime fechaAlta { get; set; }
        public DateTime? fechaBaja { get; set; }

        public DateTime fechaServidor { get; set; }

        //Relacion con la entidad cEmpresa
        public int? idEmpresa { get; set; } //FK a la tabla cEmpresa
        [ForeignKey("idEmpresa")]

        // Relación con la entidad sUsuario (uno a uno)
        public int? idUsuario { get; set; }  // FK a la tabla sUsuario
        [ForeignKey("idUsuario")]
        public virtual sUsuario Usuario { get; set; }  // Propiedad de navegación para acceder al usuario
        // Propiedad de navegación para la relación uno a muchos con cEmpresa
        //public virtual ICollection<cSucursal> Sucursales { get; set; } // 
        public virtual cEmpresa Empresa { get; set; }
    }
}