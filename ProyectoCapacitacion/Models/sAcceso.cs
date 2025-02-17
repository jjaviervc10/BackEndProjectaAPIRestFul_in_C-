using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProyectoCapacitacion.Models
{
    [Table("sAcceso")]
    public class sAcceso
    {
        [Key]//PK
        [Column("idAcceso")]

        public int idAcceso { get; set; }

        public string nombreAcceso { get; set; }
        public int orden { get; set; }
        public Boolean activo { get; set; }

        public DateTime? fechaAlta { get; set; }//Pueder ser null

        public int? idModulo { get; set; }//FK a la tabla sModulo
        [ForeignKey("idModulo")]


        // Nueva propiedad para la relación con sAplicacion (FK)
        public int? idAplicacion { get; set; } // FK a la tabla sAplicacion
        [ForeignKey("idAplicacion")]
        public virtual sAplicacion Aplicacion { get; set; } // Relación con sAplicacion (uno a muchos)

        //Relacion con la entidad idModulo (uno a uno)
        public virtual sModulo Modulo { get; set; }  // Propiedad de navegación a sModulo
  
       public virtual ICollection<sPerfil> PerfilesA { get; set; }

        public virtual ICollection<rPerfilAcceso> RPerfilAccesosA { get; set; }
    }
  
}
