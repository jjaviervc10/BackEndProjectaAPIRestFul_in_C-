namespace ProyectoCapacitacion.DTOs
{
    public class EmpresaDTO
    {
        public int idEmpresa { get; set; }
        public string nombreEmpresa { get; set; }

        public string claveEmpresa { get; set; }

        // Información solo de idUsuario
        public int? idUsuario { get; set; }

        public bool activo {  get; set; }

        public DateTime fechaAlta { get; set; }
        
        public DateTime? fechaBaja { get; set; }//Puede ser null no esta dado de baja

        public DateTime fechaServidor { get; set; } 

        //  public UsuarioDTO Usuario { get; set; }  // Solo devolvemos la relación con Usuario, que contiene el idUsuario.

  
    }
}
