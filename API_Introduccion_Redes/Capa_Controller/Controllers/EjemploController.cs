using Capa_Logica;
using Capa_Logica.Orquestador;
using Capa_Modelo.Persona;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capa_Controller.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EjemploController : Controller
    {
        private OrquestadorPersonas orquestador = new OrquestadorPersonas();

        [HttpGet("Prueba")]
        public IActionResult Prueba()
        {
            return Ok(orquestador.prueba());
        }

    }
}
