using Capa_Logica;
using Microsoft.AspNetCore.Mvc;

namespace Capa_Controller.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EjemploController : Controller
    {
        [HttpGet]
        [Route("Index")]
        public IActionResult Index() {
            return Ok();
        }
    }
}
