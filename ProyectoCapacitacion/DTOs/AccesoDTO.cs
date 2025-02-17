namespace ProyectoCapacitacion.DTOs
{
    public class AccesoDTO
    {
        public int idAcceso {  get; set; }
      
        public string nombreAcceso { get; set; }
        public int orden {  get; set; }    
        public Boolean activo { get; set; }

        public DateTime fechaAlta { get; set; }

        public int? idModulo { get; set; }

        public int? idAplicacion { get; set; }   
    }
}
