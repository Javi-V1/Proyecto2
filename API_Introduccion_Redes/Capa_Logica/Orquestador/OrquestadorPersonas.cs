using Capa_Acceso_Datos.Txt;
using Capa_Logica.Ayudante;
using Capa_Logica.ECC;
using Capa_Modelo.Persona;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
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
        List<string> passkey = new List<string>();
        public OrquestadorPersonas()
        {
            ayudante = new Ayudante_JSON();
            escritura = new Escritura_Txt(); 
            lectura = new Lectura_Txt();
            servidor = new Servidor();
            usuario = new Usuario(servidor.servidorPublicKey);
        }
        
        public bool ProcesarUsuarios()
        {
            try
            {
                List<Persona> Listapersonas1 = new List<Persona>();
                List<Persona> Listapersonas2 = new List<Persona>();
                List<Persona> Listapersonas3 = new List<Persona>();
                DividirLista(Listapersonas1, Listapersonas2, Listapersonas3);
                EncriptarListas(Listapersonas1, Listapersonas2, Listapersonas3);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void EncriptarListas(List<Persona> Listapersonas1, List<Persona> Listapersonas2, List<Persona> Listapersonas3)
        {
            Thread th2 = new Thread(() =>
            {
                EncriptarLista(Listapersonas1, "lista1encrypted.txt");
                
            });

            Thread th3 = new Thread(() =>
            {
                EncriptarLista(Listapersonas2, "lista2encrypted.txt");
                
            });

            Thread th4 = new Thread(() =>
            {
                EncriptarLista(Listapersonas3, "lista3encrypted.txt");
                
            });

            th2.Start();
            th3.Start();
            th4.Start();

            th2.Join();
            th3.Join();
            th4.Join();

        }
        private void DividirLista(List<Persona> Listapersonas1, List<Persona> Listapersonas2, List<Persona> Listapersonas3)
        {
            List<Persona> personas = AccederUsuarios();

            bool finalizarTh = false;

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
                if (finalizarTh)
                {
                    return;
                }

            });

            th1.Start();
            finalizarTh = true;
            th1.Join();
        }
        

        public bool LoginAdmin(string sharedKey)
        {
            try
            {
                byte[] sharedKeyb = Convert.FromBase64String(sharedKey);
                if (servidor.claves.Contains(sharedKeyb))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public bool LoginNormal(string user, string password)
        {
            List<Persona> personas = MostrarListas();
            foreach(Persona persona in personas)
            {
                if(user == persona.user && password == persona.password && persona.user!=null && persona.password !=null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CambiarPassword(string user, string password)
        {
            List<Persona> personas = MostrarListas();
            foreach (Persona persona in personas)
            {
                if (user == persona.user && password == persona.password)
                {
                    persona.lastPassChange = DateTime.Now;
                    return true;
                }
            }
            return false;
        }

        public bool EliminarArchivo(string rutaArchivo, string nombreArchivo, string extensArchivo)
        {
            string direccion = (rutaArchivo + nombreArchivo + extensArchivo);
            try
            {
                if (File.Exists(direccion))
                {
                    File.Delete(direccion);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public List<Persona> MostrarListas()
        {
            
             List<Persona> tpersonas = new List<Persona>();
             string contenido = lectura.Lee_Archivo("../lista1encrypted.txt");
             byte[] listaEncriptada1 = Convert.FromBase64String(contenido);
             List<Persona> temp1 = ayudante.Deserialize_Modelo<List<Persona>>(DecriptarLista(listaEncriptada1));

             contenido = lectura.Lee_Archivo("../lista2encrypted.txt");
             byte[] listaEncriptada2 = Convert.FromBase64String(contenido);
             List<Persona> temp2 = ayudante.Deserialize_Modelo<List<Persona>>(DecriptarLista(listaEncriptada2));

             contenido = lectura.Lee_Archivo("../lista3encrypted.txt");
             byte[] listaEncriptada3 = Convert.FromBase64String(contenido);
             List<Persona> temp3 = ayudante.Deserialize_Modelo<List<Persona>>(DecriptarLista(listaEncriptada3));

             tpersonas.AddRange(temp1);
             tpersonas.AddRange(temp2);
             tpersonas.AddRange(temp3);

             return tpersonas;
            
        }

        private string DecriptarLista(byte[] mensajeEncriptado)
        {
            try
            {
                string ivS = lectura.Lee_Archivo("../iv.txt");
                byte[] iv = Convert.FromBase64String(ivS);
                foreach (byte[] clave in servidor.claves)
                {
                    try
                    {
                        string contenido = usuario.Receive(mensajeEncriptado, iv, servidor.key);
                        return contenido;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void EncriptarLista(List<Persona> lista, string rutaArchivo)
        {
            try
            {
                string listaE = ayudante.Serialice_Modelo(lista);
                byte[] encrypted = servidor.EncryptMessage(listaE, usuario.usuarioPublicKey, out byte[] iv);
                string contenido = Convert.ToBase64String(encrypted);
                escritura.Escriba_En_TxT(contenido, "../", rutaArchivo);

                escritura.Escriba_En_TxT(Convert.ToBase64String(iv), "../", "iv.txt");

                servidor.GuardarClave();

            }
            catch (Exception)
            {

                throw;
            }
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
