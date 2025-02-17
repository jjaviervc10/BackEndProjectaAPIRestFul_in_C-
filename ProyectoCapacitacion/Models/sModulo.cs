using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProyectoCapacitacion.Models
{
    [Table("sModulo")]//Nombre real como esta en la base de datos
    public class sModulo
    {
        [Key]
        [Column("idModulo")]

        public int idModulo { get; set; }
        public string nombreModulo { get; set; }
        public int orden { get; set; }

        public Boolean activo { get; set; }

        public DateTime? fechaBaja { get; set; }  // Puede ser nula si no está dada de baja

        public int? idAplicacion { get; set; }  //FK a la tabla sAplicacion
        [ForeignKey("idAplicacion")]

        public virtual ICollection<sAcceso> Accesos { get; set; }

        [ForeignKey("idAplicacion")]
        public virtual sAplicacion Aplicacion { get; set; } // Propiedad de navegación para la relación con la entidad sAplicacio

    }
}
