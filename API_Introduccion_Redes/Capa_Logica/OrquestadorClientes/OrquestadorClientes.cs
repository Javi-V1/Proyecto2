using Capa_Acceso_Datos.DB;
using Capa_Acceso_Datos.Txt;
using Capa_Logica.Ayudante;
using Capa_Modelo.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica.OrquestadorClientes
{
    public class OrquestadorClientes
    {
        private Ayundate_JSON ayundate;
        private Lectura_Txt lectura;
        private Escritura_Txt escritura;
        private DB_SQL_S db;

        public OrquestadorClientes()
        {
            ayundate = new Ayundate_JSON();
            lectura = new Lectura_Txt();
            escritura = new Escritura_Txt();
            db = new DB_SQL_S();
        }

        public List<Cliente> ObtengaClientes() {

            List <Cliente> clientes = new List<Cliente>();

            /*
            clientes = db.ObtengaClientes_DB();
            */

            string contenido = lectura.Lee_Archivo("../Clientes.txt");
            Cliente clienteleido = ayundate.Deserialize_Modelo<Cliente>(contenido);
            clientes.Add(clienteleido);
            

            return clientes;
        
        }

        public bool AgregueCliente(Cliente cliente) {

            try
            {
                Cliente clientes = new Cliente();
                string jsonClientes = ayundate.Serialice_Modelo(clientes);
                escritura.Escriba_En_TxT(jsonClientes, "../", "Clientes1.txt");

                return true;
            }
            catch (Exception)
            {

                return false;
            }        
        }
    }
}
