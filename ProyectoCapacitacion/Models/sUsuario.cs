using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCapacitacion.Models
{
    [Table("sUsuario")] // Asegúrate de que esto coincida con el nombre real de la tabla
    public class sUsuario
    {

        [Key] // Marca idUsuario como la clave primaria
        public int idUsuario { get; set; }

        public string usuario { get; set; }
        public string pass { get; set; }
        public string nombreCompleto { get; set; }

        
        public virtual ICollection<cSucursal> SucursalesU {  get; set; }    

        // Relación con la entidad cEmpresa (uno a uno)
        // Propiedad de navegación para la relación uno a muchos con cEmpresa
        public virtual ICollection<cEmpresa> Empresas { get; set; } // Relación uno a muchos
       // public cEmpresa Empresas { get; set; }  // Relación de navegación si un usuario tiene una empresa asociada
        public virtual ICollection<sPerfil> PerfilesU { get; set; }  
   
         public virtual ICollection <rPerfilAcceso> RPerfilAccesosU { get; set; }
    
    }
}
