using Capa_Logica;
using Capa_Logica.Orquestador;
using Capa_Modelo.Persona;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Capa_Controller.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EjemploController : Controller
    {
        private OrquestadorPersonas orquestador = new OrquestadorPersonas();

        [HttpGet("Encript")]
        public IActionResult Encript()
        {
            byte[] mensajeencriptado = orquestador.pruebaEncrypt();
            return Ok(mensajeencriptado);
        }

        [HttpPost("Decrypt")]
        public IActionResult Decrypt(byte[] mensajeEncrypt, string sharedKey)
        {
            string mensajedesencriptado = orquestador.pruebaDecrypt(mensajeEncrypt, sharedKey);
            return Ok(mensajedesencriptado);
        }
    }
}
