namespace ProyectoCapacitacion.DTOs
{
    public class SucursalDTO
    {
        public int idSucursal { get; set; }
        public string nombreSucursal { get; set; }
        public string ciudad {  get; set; }
        public string estado { get; set; }

        public Boolean? activo {  get; set; }


        public DateTime fechaAlta { get; set; }
        public DateTime? fechaBaja { get; set; }

        public DateTime fechaServidor { get; set; }
        public int? idUsuario {  get; set; }
        //Información solo de idEmpresa
        public int? idEmpresa {  get; set; }
    }
}
