using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;
using ProyectoCapacitacion.Models;
using System;

namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmpresaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todas las empresas, devolviendo solo los datos necesarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpresaDTO>>> GetEmpresas()
        {
            var empresas = await _context.Empresa
                .Select(e => new EmpresaDTO
                {
                    idEmpresa = e.idEmpresa,
                    nombreEmpresa = e.nombreEmpresa,
                    claveEmpresa = e.claveEmpresa,
                    activo = e.activo,
                    fechaBaja = e.fechaBaja,
                    fechaAlta = e.fechaAlta,
                    fechaServidor = e.fechaServidor,
                    idUsuario = e.idUsuario ?? 0 // Asegúrate de manejar el valor nullable
                })
                .ToListAsync();

            return Ok(empresas);
        }

        // Obtener una empresa por ID, devolviendo solo los datos necesarios
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpresaDTO>> GetEmpresa(int id)
        {
            var empresa = await _context.Empresa
                .Where(e => e.idEmpresa == id)
                .Select(e => new EmpresaDTO
                {
                    idEmpresa = e.idEmpresa,
                    nombreEmpresa = e.nombreEmpresa,
                    claveEmpresa = e.claveEmpresa,
                    activo = e.activo,
                    fechaBaja = e.fechaBaja ?? null,
                    fechaServidor = e.fechaServidor,
                    fechaAlta = e.fechaAlta,
                    idUsuario = e.idUsuario ?? 0 // Asegúrate de manejar el valor nullable
                })
                .FirstOrDefaultAsync();

            if (empresa == null)
            {
                return NotFound();
            }

            return Ok(empresa);
        }

     
        // Creamos la lógica para agregar una nueva empresa y retornamos las sucursales asignadas a esta
        [HttpPost("{nombreEmpresa}")]
        public async Task<ActionResult<cEmpresa>> PostEmpresa(string nombreEmpresa, int idUsuario, [FromBody] EmpresaDTO empresaDTO)
        {
            // Verificamos si el DTO de la empresa es válido
            if (empresaDTO == null)
            {
                return BadRequest("Los datos de la empresa son inválidos");
            }

            // Verificar que el idUsuario existe en la tabla sUsuario
            var usuarioExistente = await _context.Usuarios
                                                 .FirstOrDefaultAsync(u => u.idUsuario == idUsuario); // Usamos idUsuario de la URL

            if (usuarioExistente == null)
            {
                return BadRequest("El idUsuario especificado no existe.");
            }

            // Usamos el nombreEmpresa de la URL
            var nuevaEmpresa = new cEmpresa
            {
                nombreEmpresa = nombreEmpresa.Trim(), // Tomamos el nombre de la URL
                claveEmpresa = "RFC", // Establecemos claveEmpresa como "RFC"
                idUsuario = idUsuario, // Usamos el idUsuario de la URL
                fechaAlta = DateTime.Now, // Asignamos la fecha actual a fechaAlta
                fechaBaja = null, // La columna fechaBaja puede quedarse como null
                fechaServidor = DateTime.Now // Asignamos la fecha actual a fechaServidor
            };

            // Guardamos la empresa en la base de datos
            _context.Empresa.Add(nuevaEmpresa);
            await _context.SaveChangesAsync();

            // Devolvemos el DTO de la empresa
            var empresaCreada = new EmpresaDTO
            {
                idEmpresa = nuevaEmpresa.idEmpresa,
                nombreEmpresa = nuevaEmpresa.nombreEmpresa,
                claveEmpresa = nuevaEmpresa.claveEmpresa, // En este caso será "RFC"
                activo = nuevaEmpresa.activo,
                fechaAlta = nuevaEmpresa.fechaAlta,
                fechaServidor = nuevaEmpresa.fechaServidor,
                idUsuario = idUsuario // Debería ser el idUsuario que se pasó
            };

            // Retornamos la nueva empresa
            return CreatedAtAction(nameof(GetEmpresa), new { id = empresaCreada.idEmpresa }, empresaCreada);

        }

        //Generamos el endpoint para verificar si la empresa existe por su ID
        [HttpGet("verify/{id}")]
        public async Task<ActionResult<bool>>VerifyEmpresa(int id)
        {
            var empresa = await _context.Empresa.FindAsync(id);

            //Si no se encuentra la empresa, devolvemos false
            if(empresa == null)
            {
                return NotFound("La empresa no existe.");
            }

            //Si la empresa existe, devolvemos true
            return Ok(true);
        }

        
        // Lógica para eliminar una empresa por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpresa(int id)
        {
            // Paso 1: Verificar si la empresa existe
            var empresa = await _context.Empresa.FindAsync(id);

            if (empresa == null)
            {
                return NotFound("La empresa con el ID proporcionado no existe.");
            }

            // Paso 2: Verificar si la empresa está asociada a alguna sucursal
            var sucursalesAsociadas = await _context.Sucursal
                .Where(s => s.idEmpresa == id)  // Filtramos las sucursales por la empresa asociada
                .ToListAsync();

            // Si la empresa está asociada a alguna sucursal
            if (sucursalesAsociadas.Any())
            {
                // Desasociamos la empresa de las sucursales. Aquí se puede tomar la decisión de actualizar o eliminar
                foreach (var sucursal in sucursalesAsociadas)
                {
                    // Si quieres simplemente desasociar la empresa de las sucursales (poniendo idEmpresa a null)
                  //  sucursal.idEmpresa = 1;

                    // Si prefieres eliminar las sucursales asociadas (lo que eliminará la relación)
                    _context.Sucursal.Remove(sucursal);  // Descomentar si deseas eliminar la sucursal

                    // O realizar cualquier otra lógica según tus necesidades
                }

                // Guardar los cambios en las sucursales
                await _context.SaveChangesAsync();
            }

            // Paso 3: Eliminar la empresa
            _context.Empresa.Remove(empresa);

            // Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Devolvemos un mensaje de éxito
            return Ok(true);
        }


        //Creación de la lógica para editar/actualizar los detalles de una empresa 
        /*[HttpPut("{id}")]
         public async Task<ActionResult<cEmpresa>> PutEmpresa(int id, string nombreEmpresa,bool activo, string claveEmpresa, DateTime fechaAlta, DateTime fechaServidor, DateTime? fechaBaja)
         {
             // Paso 1: Verificar si el ID de la empresa existe
             var empresa = await _context.Empresa.FindAsync(id);

             if (empresa == null)
             {
                 return NotFound("La empresa con el ID proporcionado no existe.");
             }

             // Paso 2: Procedemos con la edición si la empresa existe
             bool cambiosRealizados = false;

             // Solo actualizamos los campos si el valor es distinto al actual
             if (empresa.nombreEmpresa != nombreEmpresa)
             {
                 empresa.nombreEmpresa = nombreEmpresa;
                 cambiosRealizados = true;
             }

             if (empresa.claveEmpresa != claveEmpresa)
             {
                 empresa.claveEmpresa = claveEmpresa;
                 cambiosRealizados = true;
             }

             if (empresa.activo != activo)
             {
                 empresa.activo = activo;
                 cambiosRealizados = true;
             }
             // Verificamos y actualizamos FechaAlta si es diferente
             if (empresa.fechaAlta != fechaAlta)
             {
                 empresa.fechaAlta = DateTime.Now;
                 cambiosRealizados = true;
             }

             // Verificamos y actualizamos FechaServidor si es diferente
             if (empresa.fechaServidor != fechaServidor)
             {
                 empresa.fechaServidor = DateTime.Now;
                 cambiosRealizados = true;
             }

             // Si se proporciona una nueva FechaBaja y es diferente de la actual
             if (fechaBaja.HasValue && empresa.fechaBaja != fechaBaja.Value)
             {
                 empresa.fechaBaja = fechaBaja.Value;
                 cambiosRealizados = true;
             }
             // Si no se proporciona una FechaBaja, la dejamos en null si no estaba ya
             else if (!fechaBaja.HasValue && empresa.fechaBaja != null)
             {
                 empresa.fechaBaja = null;
                 cambiosRealizados = true;
             }

             // Si no se han realizado cambios, no necesitamos hacer nada
             if (!cambiosRealizados)
             {
                 return Ok("No se realizaron cambios.");
             }

             // Cambiar el estado de la entidad a modificada
             _context.Entry(empresa).State = EntityState.Modified;

             // Guardamos los cambios en la base de datos
             await _context.SaveChangesAsync();

             // Retornamos la empresa modificada
             return Ok(empresa);
         }*/


        //Creación de la lógica para editar/actualizar los detalles de una empresa 
        [HttpPut("{idEmpresa}")]
        public async Task<IActionResult> Put(int idEmpresa, [FromBody] EmpresaDTO empresaDTO)
        {
            // Validar si el id coincide con el id de la empresa
            if (idEmpresa != empresaDTO.idEmpresa)
            {
                return BadRequest("El id de la empresa no coincide");
            }

            // Lógica para actualizar la empresa
            var empresa = await _context.Empresa.FindAsync(idEmpresa);
            if (empresa == null)
            {
                return NotFound();
            }

            empresa.nombreEmpresa = empresaDTO.nombreEmpresa;
            empresa.claveEmpresa = empresaDTO.claveEmpresa;
            empresa.activo = empresaDTO.activo;
            empresa.fechaAlta = empresaDTO.fechaAlta;
            empresa.fechaServidor = empresaDTO.fechaServidor;
            empresa.fechaBaja = empresaDTO.fechaBaja;

            _context.Empresa.Update(empresa);
            await _context.SaveChangesAsync();

            // Retornar la empresa modificada
            return Ok(empresa); // Devolvemos el objeto actualizado
        }



    }
}
