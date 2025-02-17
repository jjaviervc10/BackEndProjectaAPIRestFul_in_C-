using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;

namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuloController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ModuloController(ApplicationDbContext context)
        {
            _context = context;
        }


        //Obtener todos los Modulos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuloDTO>>>GetModulos()
        {
            var modulos = await _context.Modulo
                .Select(m => new ModuloDTO
                {
                 idModulo = m.idModulo,
                 nombreModulo = m.nombreModulo, 
                 orden = m.orden,
                 activo = m.activo,
                 idAplicacion = m.idAplicacion ?? 0

                })
                .ToListAsync();

            return Ok(modulos);
        }

        //Obtener un modulo por ID

    }
}
