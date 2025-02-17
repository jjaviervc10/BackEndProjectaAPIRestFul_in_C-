namespace ProyectoCapacitacion.DTOs
{
    public class RPerfilAccesoDTO
    {
        public int? idPerfil {  get; set; }  
        public int idAcceso {  get; set; }
        public int idUsuario {  get; set; }

        public string descripcionPerfil {  get; set; }  

        public  List<RPerfilAccesoDTO> PerfilesAccesos{ get;  set; }
    }
}
