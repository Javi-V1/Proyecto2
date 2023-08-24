using Capa_Acceso_Datos.Txt;
using Capa_Logica.Ayudante;
using Capa_Logica.ECC;
using Capa_Modelo.Persona;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica.Orquestador
{
    public class OrquestadorPersonas
    {
        public Ayudante_JSON ayudante;
        public Escritura_Txt escritura;
        public Lectura_Txt lectura;
        private Servidor servidor;
        private Usuario usuario;


        public OrquestadorPersonas()
        {
            ayudante = new Ayudante_JSON();
            escritura = new Escritura_Txt(); 
            lectura = new Lectura_Txt();
            servidor = new Servidor();
            usuario = new Usuario(servidor.servidorPublicKey);
        }
        
        public void ProcesarUsuariosConHilos()
        {
            // Leer el archivo JSON y obtener las personas
            List<Persona> personas = AccederUsuarios();

            List<Persona> Listapersonas1 = new List<Persona>();
            List<Persona> Listapersonas2 = new List<Persona>();
            List<Persona> Listapersonas3 = new List<Persona>();
            bool stopThread = false;

            Thread th1 = new Thread(() =>
            {
                for (int x = 0; x < personas.Count(); x++)
                {
                    if (x < 10)
                    {
                        Listapersonas1.Add(personas[x]);
                    }
                    else if (x < 20)
                    {
                        Listapersonas2.Add(personas[x]);
                    }
                    else
                    {
                        Listapersonas3.Add(personas[x]);
                    }
                }
                if (stopThread) // Finaliza el hilo de manera controlada
                {
                    return;
                }

            });

            th1.Start();
            stopThread = true;
            th1.Join();
        }

        public string prueba()
        {
            try
            {
                string contenido = lectura.Lee_Archivo("../Personas.json");
                List<Persona> personas = JsonConvert.DeserializeObject<List<Persona>>(contenido);
                //string prueba = Encoding.ASCII.GetString(EncriptarLista(personas));
                string prueba2 = usuario.Receive(EncriptarLista(personas), usuario.GenerarIV());
                return prueba2;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
        
        public byte[] EncriptarLista(List<Persona> lista)
        {
            string listaAEncriptar = ayudante.Serialice_Modelo(lista);
            byte[] encryptedData;
            byte[] iv;
            byte [] encrypted= Servidor.Send(usuario.usuarioKey, listaAEncriptar, out encryptedData, out iv);
            return encrypted;
        }
        
        public List<Persona> DesencriptarLista(byte[] encryptedData)
        {
            byte[] iv = null;
            string contenidoDesencriptado = usuario.Receive(encryptedData, iv);
            List<Persona> listaDesencriptada = ayudante.Deserialize_Modelo<List<Persona>>(contenidoDesencriptado);
            return listaDesencriptada;
        }

        private List<Persona> AccederUsuarios()
        {
            try
            {
                string contenido = lectura.Lee_Archivo("../Personas.json");
                Console.WriteLine(contenido);
                List<Persona> personas = ayudante.Deserialize_Modelo<List<Persona>>(contenido);
                return personas;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
            
        }
    }
}
