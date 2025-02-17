using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;
using ProyectoCapacitacion.Models;

namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
   {
        private readonly ApplicationDbContext _context;
    

      public PerfilController(ApplicationDbContext context)
       {
         _context = context;
       }
        //Obtener todos los perfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfilDTO>>> GetPerfiles()
        {
          var perfiles = await _context.Perfil
                .Select(p => new PerfilDTO
               { 
                    idPerfil = p.idPerfil,
                    nombrePerfil = p.nombrePerfil,
                    descripcionPerfil = p.descripcionPerfil,
                    activo = p.activo,
                    fechaAlta = p.fechaAlta,
                    fechaServidor   = p.fechaServidor,
                    idUsuario = p.idUsuario ?? 0 //Aseguramos manejar el valor null
                })
                .ToListAsync();

            return Ok(perfiles);

        }

        //Obtener un perfil por ID

        [HttpGet("{id}")]
        public async Task<ActionResult<PerfilDTO>> GetPerfil(int id)
        { 
        var perfil  = await _context.Perfil
                .Where(p => p.idPerfil == id)
                .Select(p => new PerfilDTO {
                    idPerfil = p.idPerfil,
                    nombrePerfil = p.nombrePerfil,
                    descripcionPerfil = p.descripcionPerfil,
                    activo = p.activo,
                    fechaAlta = p.fechaAlta,
                    fechaServidor = p.fechaServidor,
                    idUsuario = p.idUsuario ?? 0 //Aseguramos manejar el valor null
                })
              .FirstOrDefaultAsync();

            if (perfil == null)
            {
                return NotFound();

            }
            return Ok(perfil);  

        }


        //Crreamos la lógica para agregar un nuevo perfil y retornamos los perfiles asignados a este
        [HttpPost("{nombrePerfil}")]
        public async Task<ActionResult<sPerfil>> PostPerfil (string nombrePerfil, int idUsuario, [FromBody]PerfilDTO perfilDTO)
        {
            //Se verifica si el DTO  del perfil es válido
            if(perfilDTO == null)
            {
                return BadRequest("Los datos del perfil son inválidos");
            }

            //Verificar que el idUsuario existe en la tabla sUsuario
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.idUsuario == idUsuario);
        
           if(usuarioExistente == null)
            {
                return BadRequest("El idUsuario especificado no existe. ");

            }

            //Usamos el nombrePerfil de la URL
            var nuevoPerfil = new sPerfil
            {
                nombrePerfil = nombrePerfil.Trim(),
                descripcionPerfil = "jEFASO",
                activo = false,
                fechaAlta = DateTime.Now,
                fechaBaja = null,
                fechaServidor = DateTime.Now

            };

            //Guardamos el perfil en la base de datos
            _context.Perfil.Add(nuevoPerfil);
            await _context.SaveChangesAsync();

            //Devolvemos el DTO del Perfil
            var perfilCreado = new PerfilDTO
            {
                idPerfil = nuevoPerfil.idPerfil,
                nombrePerfil = nuevoPerfil.nombrePerfil,
                descripcionPerfil = nuevoPerfil.descripcionPerfil,
                activo = false,
                idUsuario = idUsuario
            };

            //Retornamos el nuevo perfil
            return CreatedAtAction(nameof(GetPerfil), new { id = perfilCreado.idPerfil }, perfilCreado);
        
        }

        //Generamos el endpoint para verificar si el perfil existe por su ID
        [HttpGet("verify/{id}")]
        public async Task<ActionResult<bool>>VerifyPerfil(int id)
        {
            var perfil = await _context.Perfil.FindAsync(id);

            //Si no se encuentra el perfil, devolvemos false
            if(perfil == null)
            {
                return NotFound("El perfil no existe. ");
            }

            //Si el perfil existe. devolvemos true
            return Ok(true);
        }


        // Lógica para eliminar un perfil por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfil(int id)
        {

            // Paso 1: Verificar si el perfil existe
            var perfil = await _context.Perfil.FindAsync(id);
             


            if (perfil == null)
            {
                return NotFound($"El perfil con el ID {id} no existe.");
            }

            //Verificar si perfil esta asociado a algun rperfilacceso
            var rperfilaccesoAsociados =  await _context.PerfilAccesos
                .Where(rpa => rpa.idPerfil == id)//Filtramos los perfiles por el rperfilacceso asociado
                .ToListAsync();



            // Paso 2: Eliminar las relaciones en la tabla intermedia 'rPerfilAcceso' asociadas a este perfil
            if (rperfilaccesoAsociados.Any())
            {
                //Desasociamos el perfil de rperfilaccesos
                foreach (var rperfilacceso in rperfilaccesoAsociados)
                {
                    //Eliminar rperfilaccesos asociados(eliminará ña relación)
                    _context.PerfilAccesos.Remove(rperfilacceso);
                }

              await _context.SaveChangesAsync();    


            }


            // Paso 4: Eliminar el perfil
            _context.Perfil.Remove(perfil);  // Eliminar el perfil de la base de datos

            // Paso 5: Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Paso 6: Devolver respuesta exitosa
            return Ok($"El perfil con ID {id} ha sido eliminado, y sus relaciones con los accesos han sido desvinculadas.");
        }


        //CREACIÓN DE LA LÓGICA PARA EDITAR/ACTUALIZAR LOS DETALLES DE UN PERFIL
        [HttpPut("{idPerfil}")]
        public async Task<ActionResult> Put (int idPerfil, [FromBody] PerfilDTO perfilDTO)
        {
            //Validar si el id coincide con el id del perfil
            if(idPerfil != perfilDTO.idPerfil)
            {
                return BadRequest("El id del perfil no coincide");
            }



            //La lógica para actualizar un perfil
            var perfil = await _context.Perfil.FindAsync(idPerfil);
            if (perfil == null)
            {
                return NotFound();
            }
            perfil.nombrePerfil = perfilDTO.nombrePerfil;
            perfil.descripcionPerfil = perfilDTO.descripcionPerfil;

            _context.Perfil.Update(perfil); 
            await _context.SaveChangesAsync();

            //Retornar el perfil modificado
            return Ok(perfil);

        }

    }


}
