using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;
using ProyectoCapacitacion.Models;


namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class SucursalController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public SucursalController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Obtener todas las sucursales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SucursalDTO>>> GetSucursales()
        {
            var sucursales = await _context.Sucursal
                .Select(s => new SucursalDTO
                {
                    idSucursal = s.idSucursal,
                    nombreSucursal = s.nombreSucursal,
                    ciudad = s.ciudad ?? "Chihhuaspos",
                    estado = s.estado ?? "Culiacas",
                    activo = s.activo ?? false,
                    fechaBaja = s.fechaBaja,
                    fechaAlta = s.fechaAlta,
                    fechaServidor = s.fechaServidor,
                    idEmpresa = s.idEmpresa ?? 5 //Manejamos el valor null


                })
                .ToListAsync();
            return Ok(sucursales);
        }

        //Obtener una Sucursal por ID, devolviendo solo los datos necesarios
        [HttpGet("{id}")]
        public async Task<ActionResult<SucursalDTO>> GetSucursal(int id)
        {
            var sucursal = await _context.Sucursal
                  .Where(s => s.idSucursal == id)
                  .Select(s => new SucursalDTO
                  {
                      idSucursal = s.idSucursal,
                      nombreSucursal = s.nombreSucursal,
                      ciudad = s.ciudad ?? "Chihhuaspos",
                      estado = s.estado ?? "Culiacas",
                      activo = s.activo ?? false,
                      idEmpresa = s.idEmpresa ?? 5
                  })
                  .FirstOrDefaultAsync();
            if (sucursal == null)
            {
                return NotFound();
            }
            return Ok(sucursal);
        }
        ///Otros metodos


        [HttpPost("{nombreSucursal}")]
        public async Task<ActionResult<SucursalDTO>> PostSucursal(string nombreSucursal, int idUsuario, int idEmpresa, [FromBody] SucursalDTO sucursalDTO)
        {
            // Verificamos si el DTO de la sucursal es válido
            if (sucursalDTO == null)
            {
                return BadRequest("Los datos de la sucursal son inválidos");
            }

            // Verificamos si la empresa con la id proporcionada existe
            var empresaID = await _context.Empresa
                .FirstOrDefaultAsync(e => e.idEmpresa == idEmpresa);
            if (empresaID == null)
            {
                return NotFound($"La empresa con id {sucursalDTO.idEmpresa} no existe.");
            }

            // Verificamos si ya existe una sucursal con el mismo nombre asociada a la misma empresa
            var sucursalExistente = await _context.Sucursal
                .Where(s => s.nombreSucursal == nombreSucursal && s.idEmpresa == idEmpresa)
                .FirstOrDefaultAsync();

            // Si existe una sucursal con el mismo nombre y la misma empresa, no se puede crear
            if (sucursalExistente != null)
            {
                return BadRequest($"Ya existe una sucursal con el nombre '{nombreSucursal}' asociada a la empresa con id {sucursalDTO.idEmpresa}. No se puede asignar la misma empresa a una sucursal con el mismo nombre.");
            }

            // Verificar que el idUsuario existe en la tabla sUsuario
            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.idUsuario == idUsuario);

            if (usuarioExistente == null)
            {
                return BadRequest("El idUsuario especificado no existe.");
            }

            // Usamos el nombreSucursal de la URL
            var nuevaSucursal = new cSucursal
            {
                nombreSucursal = nombreSucursal.Trim(),
                ciudad = "Matamorelos", // Valor predeterminado
                estado = "Qualtpatzingo", // Valor predeterminado
                idUsuario = idUsuario,
                activo = false,
                fechaAlta = DateTime.Now,
                fechaBaja = null,
                fechaServidor = DateTime.Now,
                idEmpresa = idEmpresa // Usamos el idEmpresa de la URL
            };

            // Se guarda la sucursal en la base de datos
            _context.Sucursal.Add(nuevaSucursal);
            await _context.SaveChangesAsync();

            // Devolvemos el DTO de la nueva sucursal con los datos necesarios
            var sucursalCreada = new SucursalDTO
            {
                idSucursal = nuevaSucursal.idSucursal,
                nombreSucursal = nuevaSucursal.nombreSucursal,
                ciudad = nuevaSucursal.ciudad,
                estado = nuevaSucursal.estado,
                activo = nuevaSucursal.activo ?? false,
                idUsuario = idUsuario,
                idEmpresa = idEmpresa // Se usa la idEmpresa que se proporcionó
            };

            return CreatedAtAction(nameof(GetSucursal), new { id = sucursalCreada.idSucursal }, sucursalCreada);
        }

        //Generamos el endpoint para verificar si la sucursal existe por su ID
        [HttpGet("verify/{id}")]
        public async Task<ActionResult<bool>> VerifyEmpresa(int id)
        {
            var sucursal = await _context.Sucursal.FindAsync(id);

            //Si no se encuentra la sucursal, devolvemos false
            if (sucursal == null)
            {
                return NotFound("la sucursal no existe.");
            }
            //Si la sucursal existe , se devuelve true
            return Ok(true);
        }


        //Lógca para eliminar una sucursal por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSucursal(int id)
        {
            // Paso 1: Verificar si la sucursal existe
            var sucursal = await _context.Sucursal
                .FirstOrDefaultAsync(s => s.idSucursal == id);

            if (sucursal == null)
            {
                return NotFound("La sucursal con el ID proporcionado no existe.");
            }

            // Paso 2: Obtener la empresa asociada a la sucursal
            var empresa = await _context.Empresa
                .Include(e => e.Sucursales) // Incluir las sucursales asociadas a la empresa
                .FirstOrDefaultAsync(e => e.idEmpresa == sucursal.idEmpresa);

            if (empresa != null)
            {
                // Desvinculamos la sucursal de la empresa
                empresa.Sucursales.Remove(sucursal); // Eliminar la sucursal de la colección de sucursales de la empresa

                // Actualizamos la sucursal y la empresa en la base de datos
                sucursal.idEmpresa = null; // Desvinculamos la sucursal de la empresa
                _context.Update(sucursal);
                _context.Update(empresa); // Actualizamos la empresa también
            }

            // Paso 3: Eliminar la sucursal
            _context.Sucursal.Remove(sucursal); // Eliminar la sucursal de la base de datos
            await _context.SaveChangesAsync();  // Guardar los cambios en la base de datos

            // Paso 4: Devolver respuesta exitosa
            return Ok($"La sucursal con ID {id} ha sido eliminada y su asociación con la empresa ha sido desvinculada.");
        }


        //Loógica para actualizar un registro por su ID de sucursal
        [HttpPut("{idSucursal}")]
        public async Task<IActionResult> Put (int idSucursal, [FromBody] SucursalDTO sucursalDTO)
        {
            //Realizar validación del idSucursal
            // return idSucursal != sucursalDTO.idSucursal ? BadRequest("El id de la sucursal no coincide") : null;

            if (idSucursal != sucursalDTO.idSucursal)
            {
                return BadRequest("El id de la sucursal  no coincide");
            }

            var sucursal = await _context.Sucursal
                           .FirstOrDefaultAsync(s => s.idSucursal == idSucursal);


            //Lógica Paso 2: Obtener la empresa asociada a la sucursal
            var empresa = await _context.Empresa
                .Include(e => e.Sucursales)//Incluir las sucursales asociadas a la empresa
                .FirstOrDefaultAsync(e => e.idEmpresa == sucursal.idEmpresa) ;


            if (sucursal == null)
            { 
             return NotFound(); 
            }

            sucursal.nombreSucursal = sucursalDTO.nombreSucursal;
            sucursal.ciudad = sucursalDTO.ciudad;
            sucursal.estado = sucursalDTO.estado;
            sucursal.activo = sucursalDTO.activo;  
            sucursal.fechaAlta = sucursalDTO.fechaAlta;
            sucursal.fechaBaja = sucursalDTO.fechaBaja;
            sucursal.fechaServidor = sucursalDTO.fechaServidor;

            _context.Update(sucursal);
            _context.Update(empresa);

            _context.Sucursal.Update(sucursal);

            await _context.SaveChangesAsync();

            return Ok(true);
        }



    }
}
