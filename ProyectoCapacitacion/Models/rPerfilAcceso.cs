using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ProyectoCapacitacion.Models
{
    [Table("rPerfilAcceso")]

    public class rPerfilAcceso
    {
       //FK de  idPerfil en rPerfilAcceso (realacion uno a uno)
       public int? idPerfil {  get; set; }

        //FK de idAcceso en rPerfilAcceso (realacion uno a uno)
        public int idAcceso {  get; set; }

        //FK de idUsuario en rPerfilAcceso (realacion uno a uno)
        public int? idUsuario {  get; set; }


        //public virtual ICollection<sPerfil> Perfilesperfil { get; set; }

      //  public ICollection<sAcceso> AccesoPerfiles { get; set; }

        public virtual sAcceso AccesoPerf { get; set; }//Propiedad de navegacion para acceder a acceso
    
        public virtual sUsuario UsuarioPerf { get; set; }//Propiedad de navegacion para acceder a usuario

        public virtual sPerfil PerfilrPerfil { get; set; }//Propiedad de navegacion para acceder a perfiles
    }
}
