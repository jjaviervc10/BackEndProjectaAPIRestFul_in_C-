namespace ProyectoCapacitacion.DTOs
{
    public class PerfilDTO
    {
        public int idPerfil {  get; set; }  

        public string nombrePerfil {  get; set; }   

        public string descripcionPerfil {  get; set; }

        public Boolean? activo { get; set; }


        public DateTime fechaAlta { get; set; }
        public DateTime? fechaBaja { get; set; }

        public DateTime fechaServidor { get; set; }

        // public DateTime fechaAlta { get; set; }
        public int? idUsuario {  get; set; }


    }
}
