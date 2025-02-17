namespace ProyectoCapacitacion.DTOs
{
    public class ModuloDTO
    {
      public int idModulo {  get; set; }    
        public string nombreModulo { get; set; }
        public int orden {  get; set; } 

        public Boolean activo {  get; set; }

        public int idAplicacion { get; set; }
    }
}
