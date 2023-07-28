using Capa_Logica;
using Capa_Logica.OrquestadorClientes;
using Capa_Modelo.Cliente;
using Microsoft.AspNetCore.Mvc;

namespace Capa_Controller.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EjemploController : Controller
    {
        [HttpGet]
        [Route("ObtengaClientesDisponibles")]
        public IActionResult ObtengaClientesDisponibles() {
            OrquestadorClientes orquestador = new OrquestadorClientes();
            List<Cliente> clientes = orquestador.ObtengaClientes();
            return Ok(clientes);
        }

        [HttpPost]
        [Route("AgregueCliente")]
        public IActionResult AgregueCliente(Cliente cliente){

            OrquestadorClientes orquestador = new OrquestadorClientes();
            bool resultado = orquestador.AgregueCliente(cliente);
            if (resultado)
            {
                return Ok();
            }
            else {
                return BadRequest();
            }
        }
    }
}
