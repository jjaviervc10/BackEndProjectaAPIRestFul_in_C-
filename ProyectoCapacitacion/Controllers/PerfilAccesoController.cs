using Microsoft.AspNetCore.Mvc; // Mantener solo esta importación
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;
using ProyectoCapacitacion.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilAccesoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfilAccesoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todos los rPerfilAccesos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RPerfilAccesoDTO>>> GetRPerfilAccesos()
        {
            var rperfilaccesos = await _context.PerfilAccesos
                .Select(rpa => new RPerfilAccesoDTO
                {
                    idAcceso = rpa.idAcceso,
                    idPerfil = rpa.idPerfil,
                    idUsuario = rpa.idUsuario ?? 0 // Manejamos valor null
                })
                .ToListAsync();

            return Ok(rperfilaccesos);
        }


        [HttpPost("{idPerfil}/{idAcceso}/{idUsuario}")]
        public async Task<ActionResult<RPerfilAccesoDTO>> PostRPerfilAcceso(int idPerfil, int idAcceso, int idUsuario)
        {
            // Verificar que el idUsuario existe en la tabla SUsuario
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.idUsuario == idUsuario);

            if (usuarioExistente == null)
            {
                return BadRequest("El idUsuario especificado no existe.");
            }

            // Verificar que el idPerfil existe en la tabla SPerfil
            var perfilExistente = await _context.Perfil
                .FirstOrDefaultAsync(p => p.idPerfil == idPerfil);

            if (perfilExistente == null)
            {
                return BadRequest("El idPerfil especificado no existe.");
            }

            // Verificar que el idAcceso existe en la tabla SAcceso
            /* var accesoExistente = await _context.Acceso
                 .FirstOrDefaultAsync(a => a.idAcceso == idAcceso);

             if (accesoExistente == null)
             {
                 return BadRequest("El idAcceso especificado no existe.");
             }*/

            // Verificar si el rPerfilAcceso con el idPerfil e idAcceso ya existe
            var rPerfilAccesoExistente = await _context.PerfilAccesos
                .Where(rpa => rpa.idPerfil == idPerfil && rpa.idAcceso == idAcceso)
                .FirstOrDefaultAsync();

            if (rPerfilAccesoExistente != null)
            {
                return BadRequest($"Ya existe un rPerfilAcceso con el idPerfil '{idPerfil}' e idAcceso '{idAcceso}'.");
            }

            // Crear el nuevo rPerfilAcceso con los datos del DTO
            var nuevoRPerfilAcceso = new rPerfilAcceso
            {
                idPerfil = idPerfil,
                idAcceso = idAcceso,
                idUsuario = idUsuario
            };

            // Guardar el nuevo rPerfilAcceso en la base de datos
            _context.PerfilAccesos.Add(nuevoRPerfilAcceso);
            await _context.SaveChangesAsync();

            // Retornar el DTO del nuevo rPerfilAcceso
            var rPerfilAccesoDTO = new RPerfilAccesoDTO
            {
                idPerfil = idPerfil,
                idAcceso = idAcceso,
                idUsuario = idUsuario
            };

            return CreatedAtAction(nameof(GetRPerfilAccesos), new { idPerfil = idPerfil, idAcceso = idAcceso, idUsuario = idUsuario }, rPerfilAccesoDTO);
        }
        [HttpGet("accesosDisponibles/{idPerfil}")]
        public async Task<ActionResult<IEnumerable<AccesoDTO>>> GetAccesosDisponibles(int idPerfil)
        {
            var accesosDisponibles = await _context.Acceso
         .Include(a => a.Modulo)  // Asegúrate de incluir la relación con Modulo
         .Where(a => !_context.PerfilAccesos
             .Where(rpa => rpa.idPerfil == idPerfil)
             .Select(rpa => rpa.idAcceso)
             .Contains(a.idAcceso)) // Filtrar los accesos que no están asignados
         .Select(a => new AccesoDTO
         {
             idAcceso = a.idAcceso,
             nombreAcceso = a.nombreAcceso,
             idAplicacion = a.Modulo != null ? a.Modulo.idAplicacion : (int?)null // Asegurarse de que Modulo no sea null
         })
         .ToListAsync();

            return Ok(accesosDisponibles);
        }


        [HttpGet("accesosAsignados/{idPerfil}")]
        public async Task<ActionResult<IEnumerable<AccesoDTO>>> GetAccesosAsignados(int idPerfil)
        {
            // Crear la subconsulta para obtener los accesos asignados al perfil
            var accesosAsignadosSubquery = _context.PerfilAccesos
                .Where(rpa => rpa.idPerfil == idPerfil)
                .Select(rpa => rpa.idAcceso);

            // Obtener los accesos cuyos ids estén en la subconsulta
            var accesos = await _context.Acceso
                .Where(a => accesosAsignadosSubquery.Contains(a.idAcceso))  // Subconsulta en el Where
                .Include(a => a.Modulo)  // Incluir la relación con Módulo
                .Select(a => new AccesoDTO
                {
                    idAcceso = a.idAcceso,
                    nombreAcceso = a.nombreAcceso,
                    orden = a.orden,
                    activo = a.activo,
                    idModulo = a.idModulo,
                    idAplicacion = a.Modulo != null ? a.Modulo.idAplicacion : (int?)null // Si el Módulo es null, idAplicacion también será null
                })
                .ToListAsync();

            return Ok(accesos);
        }



        // Eliminar un acceso de un perfil (desasignar)
        [HttpDelete("{idPerfil}")]
        public async Task<IActionResult> DeleteDesasignarAcceso(int idPerfil, int idAcceso)
        {
            // Buscar el rPerfilAcceso con el idPerfil e idAcceso
            var rPerfilAcceso = await _context.PerfilAccesos
                .Where(rpa => rpa.idPerfil == idPerfil && rpa.idAcceso == idAcceso)
                .FirstOrDefaultAsync();

            if (rPerfilAcceso == null)
            {
                return NotFound($"No se encontró el rPerfilAcceso con idPerfil '{idPerfil}' y idAcceso '{idAcceso}'.");
            }

            // Eliminar la relación
            _context.PerfilAccesos.Remove(rPerfilAcceso);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna sin contenido para indicar que fue exitoso
        }



        // Actualizar los accesos asignados de un perfil (método PUT)
        [HttpPut("updateAccesos/{idPerfil}/{idUsuario}")]
        public async Task<IActionResult> UpdateAccesosAsignados(int idPerfil, [FromBody] List<int> accesosAsignados, int idUsuario)
        {
            var perfilExistente = await _context.Perfil
                .FirstOrDefaultAsync(p => p.idPerfil == idPerfil);

            if (perfilExistente == null)
            {
                return NotFound($"El perfil con idPerfil '{idPerfil}' no existe.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Eliminar los accesos existentes
                    var accesosExistentes = await _context.PerfilAccesos
                        .Where(rpa => rpa.idPerfil == idPerfil)
                        .ToListAsync();

                    _context.PerfilAccesos.RemoveRange(accesosExistentes);

                    // Usamos parámetros dinámicos para la consulta
                    var parameters = string.Join(",", accesosAsignados.Select((x, i) => $"@p{i}").ToArray());

                    // Ejecutar consulta SQL manualmente para verificar los accesos válidos
                    var accesosValidos = await _context.Acceso
                        .FromSqlRaw($@"
                    SELECT a.idAcceso, a.nombreAcceso, m.idAplicacion
                    FROM sAcceso a
                    LEFT JOIN sModulo m ON a.idModulo = m.idModulo
                    WHERE a.idAcceso IN ({parameters})",
                            accesosAsignados.Select((x, i) => new Microsoft.Data.SqlClient.SqlParameter($"@p{i}", x)).ToArray()) // Pasar los parámetros
                        .Select(a => new AccesoDTO
                        {
                            idAcceso = a.idAcceso,
                            nombreAcceso = a.nombreAcceso,
                            idAplicacion = a.idAplicacion
                         
                        })
                        .ToListAsync();

                    if (accesosValidos.Count != accesosAsignados.Count)
                    {
                        return BadRequest("Uno o más accesos no son válidos.");
                    }

                    // Crear los nuevos accesos y asignarlos al perfil
                    var nuevosAccesos = accesosAsignados.Select(idAcceso => new rPerfilAcceso
                    {
                        idPerfil = idPerfil,
                        idAcceso = idAcceso,
                        idUsuario = idUsuario
                       
                    }).ToList();

                    _context.PerfilAccesos.AddRange(nuevosAccesos);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return NoContent();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Error al actualizar los accesos asignados: {ex.Message}");
                }
            }
        }




    }

}

