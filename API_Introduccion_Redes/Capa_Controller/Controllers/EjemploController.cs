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
        private OrquestadorPersonas orquestador;

        public EjemploController()
        {
            orquestador = new OrquestadorPersonas();
        }
        [HttpGet("ProcesarUsuarios")]
        public IActionResult ProcesarUsuarios()
        {
            bool result = orquestador.ProcesarUsuarios();
            return result ? Ok("Se procesaron correctamente los datos") : BadRequest();
        }
        
        [HttpGet("LoginAdmin")]
        public IActionResult LoginAdmin(string sharedKey)
        {
            bool result = orquestador.LoginAdmin(sharedKey);
            try
            {
                if (result == true)
                {
                    return Ok(MostrarLista());
                }
                else
                {
                    return BadRequest();
                }
                
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("MostrarLista")]
        private IActionResult MostrarLista()
        {
            List<Persona> ListaPersonas = orquestador.MostrarListas();
            return Ok(ListaPersonas);
        }

        [HttpPost("LoginNormal")]
        public IActionResult LoginNormal(string user, string password)
        {
            bool result = orquestador.LoginNormal(user, password);
            return result? Ok("Acceso concedido, Bievenido: " + user) : Unauthorized("Acceso rechazado");
        }

        [HttpPut("CambiarPassword")]
        public IActionResult CambiarPassword(string user, string password)
        {
            bool result = orquestador.CambiarPassword(user, password);
            return result ? Ok("Se cambio al contrasenna satisfactoriamente") : BadRequest();
        }

        [HttpDelete("EliminarArchivo")]
        public IActionResult EliminarArchivo(string rutaArchivo, string nombreArchivo, string extensArchivo)
        {
            bool result = orquestador.EliminarArchivo(rutaArchivo, nombreArchivo, extensArchivo);
            return result ? Ok("Se ha eliminado un archivo") : BadRequest();
        }
    }
}
