using Capa_Logica;
using Capa_Logica.OrquestadorClientes;
using Capa_Logica.OrquestadorUsuarios;
using Capa_Modelo.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capa_Controller.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EjemploController : Controller
    {
        private OrquestadorUsuarios orquestador = new OrquestadorUsuarios();

    }
}
