using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoCapacitacion.Data;
using ProyectoCapacitacion.DTOs;
using ProyectoCapacitacion.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

namespace ProyectoCapacitacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new UsuarioDTO
                {
                    idUsuario = u.idUsuario,
                    usuario = u.usuario,
                    pass = u.pass,
                    nombreCompleto = u.nombreCompleto
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.idUsuario == id)
                .Select(u => new UsuarioDTO
                {
                    idUsuario = u.idUsuario,
                    usuario = u.usuario,
                    pass = u.pass,
                    nombreCompleto = u.nombreCompleto
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // Otros métodos POST, PUT y DELETE...



        // Método para comparar credenciales de login (POST)
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Login([FromBody] UsuarioDTO loginDto)
        {
            // Busca el usuario por el nombre de usuario
            var usuario = await _context.Usuarios
                .Where(u => u.usuario == loginDto.usuario)
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado");
            }

            // Verifica si la contraseña cifrada coincide
            if (!VerificarContraseña(loginDto.pass, usuario.pass))
            {
                return Unauthorized("Contraseña incorrecta");
            }

            // Si la autenticación es exitosa, crea el token JWT
            var token = GenerateJwtToken(usuario);

            // Si la autenticación es exitosa, devuelve los detalles del usuario
            var usuarioDto = new UsuarioDTO
            {
                idUsuario = usuario.idUsuario,
                usuario = usuario.usuario,
                pass = usuario.pass,
                nombreCompleto = usuario.nombreCompleto
            };

            return Ok(new { token, usuario = usuarioDto }); // Retorna la información del usuario
        }

        // Método para verificar la contraseña usando SHA-1
        private bool VerificarContraseña(string contraseñaIngresada, string contraseñaAlmacenada)
        {
            // Convertir la contraseña ingresada en un hash SHA-1
            string hashedInput = HashPassword(contraseñaIngresada);

            // Comparar el hash de la contraseña ingresada con el hash almacenado
            return hashedInput == contraseñaAlmacenada;
        }

        // Método para generar el hash de la contraseña con SHA-1
        private string HashPassword(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                // Convertir la contraseña en bytes y calcular el hash
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha1.ComputeHash(passwordBytes);

                // Convertir el hash a un string hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashedBytes)
                {
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        // Método para generar el JWT
        private string GenerateJwtToken(sUsuario usuario)
        {
            // Validación detallada de los datos del usuario
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "El objeto usuario no puede ser nulo.");
            }

            if (string.IsNullOrEmpty(usuario.usuario))
            {
                throw new ArgumentNullException(nameof(usuario.usuario), "El nombre de usuario no puede ser nulo o vacío.");
            }

            if (usuario.idUsuario == 0)
            {
                throw new ArgumentNullException(nameof(usuario.idUsuario), "El ID de usuario no puede ser 0.");
            }

            // Verificación de que la clave secreta está configurada
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("La clave secreta JWT no está configurada.");
            }

            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryDurationMinutes = Convert.ToInt32(_configuration["JwtSettings:ExpiryDurationMinutes"]);

            // Crea la clave de seguridad
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Crear los claims del token
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.usuario),  // Nombre de usuario
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // Identificador único
        new Claim("idUsuario", usuario.idUsuario.ToString())  // ID del usuario
    };

            // Generar el token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryDurationMinutes),
                signingCredentials: credentials
            );

            // Retorna el token generado en formato string
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}


