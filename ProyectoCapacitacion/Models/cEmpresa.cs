using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCapacitacion.Models
{
    [Table("cEmpresa")] // Asegúrate de que esto coincida con el nombre real de la tabla
    public class cEmpresa
    {
        [Key] // Marca esta propiedad como la clave primaria
        [Column("idEmpresa")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Autoincremental
        public int idEmpresa { get; set; }  // Clave primaria
        public string claveEmpresa { get; set; }
        public string nombreEmpresa { get; set; }
        public bool activo { get; set; }
        public DateTime fechaAlta { get; set; }
        public DateTime? fechaBaja { get; set; }  // Puede ser nula si no está dada de baja
        public DateTime fechaServidor { get; set; }

        // Relación con la entidad sUsuario (uno a uno)
        public int? idUsuario { get; set; }  // FK a la tabla sUsuario
        [ForeignKey("idUsuario")]

        // Relación con la entidad sUsuario (navegación)
        public virtual ICollection<cSucursal> Sucursales { get; set; } // 
        public virtual sUsuario Usuario { get; set; }  // Propiedad de navegación para acceder al usuario
    }
}
