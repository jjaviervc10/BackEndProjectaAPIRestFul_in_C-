using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCapacitacion.Models
{
    [Table("sPerfil")]
    public class sPerfil
    {
        [Key]
        [Column("idPerfil")]

        public int idPerfil { get; set; }
        public string nombrePerfil {  get; set; }
        public string descripcionPerfil {  get; set; }
        public Boolean activo {  get; set; }
        public DateTime fechaAlta { get; set; }

        public DateTime? fechaBaja { get; set; }  // Puede ser nula si no está dada de baja
        public DateTime fechaServidor { get; set; }


        //Relación con la entidad sUsuario (uno a uno)
        [ForeignKey("idUsuario")]
        public int? idUsuario { get; set; }
        public virtual sUsuario Usuario { get; set; }  // Propiedad de navegación para acceder al usuario

        // Relación muchos a muchos con sAcceso (un perfil puede tener muchos accesos)
        public virtual ICollection<sAcceso> Accesos { get; set; } // Colección de accesos asignados a este perfil

      public virtual ICollection<rPerfilAcceso> RPerfilAccesosP { get; set; }

      

    }
}
