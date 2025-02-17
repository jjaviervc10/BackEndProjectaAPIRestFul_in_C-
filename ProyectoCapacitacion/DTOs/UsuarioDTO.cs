namespace ProyectoCapacitacion.DTOs
{
    public class UsuarioDTO
    {
        public int idUsuario { get; set; }  // Solo devolvemos el idUsuario, o cualquier otro campo que necesites.
        public string usuario { get; set; }
        public string pass {  get; set; }   
        public string nombreCompleto { get; set; }
    }
}

