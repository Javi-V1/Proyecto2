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

            string contenido = lectura.Lee_Archivo("../Clientes.txt");
            clientes = ayundate.Deserialize_Modelo<List<Cliente>>(contenido);     

            return clientes;
        
        }

        public bool AgregueCliente(Cliente cliente) {

            try
            {
                List<Cliente> clientes = ObtengaClientes();
                clientes.Add(cliente);
                string jsonClientes = ayundate.Serialice_Modelo(clientes);
                escritura.Escriba_En_TxT(jsonClientes, "../", "Clientes.txt");

                return true;
            }
            catch (Exception)
            {

                return false;
            }        
        }

        public bool Login(string nombre, string identificacion) {

            List<Cliente> clientes = ObtengaClientes();

            foreach (Cliente cliente in clientes)
            {
                if (cliente.Nombre == nombre && cliente.Identificacion == identificacion)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ActualiceCliente(string identificacion,string apellido2) {
            try
            {
                List<Cliente> clientes = ObtengaClientes();
                foreach (Cliente cliente in clientes)
                {

                    if (cliente.Identificacion == identificacion)
                    {
                        cliente.Apellido_2 = apellido2;
                    }
                }
                string jsonClientes = ayundate.Serialice_Modelo(clientes);
                escritura.Escriba_En_TxT(jsonClientes, "../", "Clientes.txt");
                return true;
            }
            catch (Exception)
            {

                return false;
            }           
        }

        public bool BorreCLiente(string identificacion) {
            try
            {
                List<Cliente> clientes = ObtengaClientes();
                Cliente clienteABorrar = new Cliente();
                foreach (Cliente cliente in clientes)
                {

                    if (cliente.Identificacion == identificacion)
                    {
                        clienteABorrar = cliente;
                    }
                }
                if (clienteABorrar != null)
                {
                    clientes.Remove(clienteABorrar);
                }

                string jsonClientes = ayundate.Serialice_Modelo(clientes);
                escritura.Escriba_En_TxT(jsonClientes, "../", "Clientes.txt");
                return true;
            }
            catch (Exception)
            {

                return false;
            }
          
        }
    }
}
