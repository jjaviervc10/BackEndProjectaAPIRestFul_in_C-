using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;


namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AplicacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AplicacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Obtener todas las aplicaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AplicacionDTO>>> GetAplicacion()
        {
            var aplicaciones = await _context.Aplicacion
                  .Select(ap => new AplicacionDTO
                  {
                      idAplicacion = ap.idAplicacion,
                      nombreAplicacion = ap.nombreAplicacion,
                      orden = ap.orden,
                      activo = ap.activo
                      //fechaAlta = a.fechaAlta.cast(DateTime.Now)
                  })
                  .ToListAsync();

            return Ok(aplicaciones);
        }


    }
}
