using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProyectoCapacitacion.Models
{
    [Table("sAplicacion")]
    public class sAplicacion
    {
        [Key]
        [Column("idAplicacion")]
        public int idAplicacion { get; set; }//PK
        public string nombreAplicacion { get; set; }
        public int orden { get; set; }
        public Boolean activo { get; set; }
        public DateTime? fechaAlta { get; set; }


        public virtual ICollection<sModulo> Modulos { get; set; } // 
       // public virtual ICollection<sAplicacion> Aplicacion {  get; set; }
    }
}
