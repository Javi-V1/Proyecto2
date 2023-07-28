using Capa_Logica;
using Capa_Logica.OrquestadorClientes;
using Capa_Modelo.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capa_Controller.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EjemploController : Controller
    {
        private OrquestadorClientes orquestador = new OrquestadorClientes();
        [HttpGet]
        [Route("ObtengaClientesDisponibles")]
        public IActionResult ObtengaClientesDisponibles() {
            List<Cliente> clientes = orquestador.ObtengaClientes();
            return Ok(clientes);
        }

        [HttpPost]
        [Route("AgregueCliente")]
        public IActionResult AgregueCliente(Cliente cliente) {
             bool resultado = orquestador.AgregueCliente(cliente);
            if (resultado)
            {
                return Ok();
            }
            else {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string nombre, string identificacion) {

            
            bool resultado = orquestador.Login(nombre, identificacion);
            return resultado ? Ok() : Unauthorized();
        }

        [HttpPut]
        [Route("ActualiceCliente")]
        public IActionResult ActualiceCliente(string identificacion, string apellido2) {
            bool resultado = orquestador.ActualiceCliente(identificacion, apellido2);
            return resultado ? Ok() : BadRequest();
        }

        [HttpDelete]
        [Route("BorreCliente")]
        public IActionResult BorreCliente(string identificacion) {
            bool resultado = orquestador.BorreCLiente(identificacion);
            return resultado ? Ok() : BadRequest();
        }
    }
}
