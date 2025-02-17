using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;


namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Obtener todos los accesos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccesoDTO>>>GetAccesos()
        {
            var accesos = await _context.Acceso
                .Select(a => new AccesoDTO
                { 
                    idAcceso = a.idAcceso,
                    idModulo = a.idModulo ?? 0,
                    nombreAcceso = a.nombreAcceso,
                    activo = a.activo,
                    orden = a.orden

                })
                .ToListAsync();
            return Ok(accesos);

        }


        //Obtener un acceso por ID
    }
}
