namespace ProyectoCapacitacion.DTOs
{
    public class AplicacionDTO
    {
      public int idAplicacion {  get; set; }
        public string nombreAplicacion { get; set; }   
        public int orden {  get; set; }
        public Boolean activo { get; set; }
        public DateTime fechaAlta { get; set; }

    }
}
