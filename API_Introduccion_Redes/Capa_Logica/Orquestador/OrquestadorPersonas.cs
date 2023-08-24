using Capa_Acceso_Datos.Txt;
using Capa_Logica.Ayudante;
using Capa_Logica.ECC;
using Capa_Modelo.Persona;
using Newtonsoft.Json;
using System;
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
        private string keyAchv1;
        private string keyAchv2;
        private string keyAchv3;
        private bool authLogIn = false;
        public OrquestadorPersonas()
        {
            ayudante = new Ayudante_JSON();
            escritura = new Escritura_Txt(); 
            lectura = new Lectura_Txt();
            servidor = new Servidor();
            usuario = new Usuario(servidor.servidorPublicKey);
        }
        
        public void ProcesarUsuarios()
        {
            List<Persona> Listapersonas1 = new List<Persona>();
            List<Persona> Listapersonas2 = new List<Persona>();
            List<Persona> Listapersonas3 = new List<Persona>();

            DividirLista(Listapersonas1, Listapersonas2, Listapersonas3);
            EncriptarListas(Listapersonas1, Listapersonas2, Listapersonas3);
        }
        private void EncriptarListas(List<Persona> Listapersonas1, List<Persona> Listapersonas2, List<Persona> Listapersonas3)
        {
            Thread th2 = new Thread(() =>
            {
                EncriptarLista(Listapersonas1, "lista1encrypted.txt");
                keyAchv1 = GenerarLlaveCompartida("lista1encrypted.txt");
            });

            Thread th3 = new Thread(() =>
            {
                EncriptarLista(Listapersonas2, "lista2encrypted.txt");
                keyAchv2 = GenerarLlaveCompartida("lista2encrypted.txt");
            });

            Thread th4 = new Thread(() =>
            {
                EncriptarLista(Listapersonas3, "lista3encrypted.txt");
                keyAchv3 = GenerarLlaveCompartida("lista3encrypted.txt");
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
        private string GenerarLlaveCompartida(string rutaArchivo)
        {
            try
            {
                byte[] llaveB = usuario.GetSharedKey();
                string llaveS = Convert.ToBase64String(llaveB);
                escritura.Escriba_En_TxT(llaveS, "../", rutaArchivo);
                return llaveS;
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        
        public bool LoginAdmin(string sharedKeyAchv1, string sharedKeyAchv2, string sharedKeyAchv3)
        {
            try
            {
                if (sharedKeyAchv1 == keyAchv1 && sharedKeyAchv2 == keyAchv2 && sharedKeyAchv3 == keyAchv3)
                {
                    authLogIn = true;
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
                if(user == persona.user && password == persona.password)
                {
                    authLogIn = true;
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
            if (authLogIn == true)
            {
                List<Persona> tpersonas = new List<Persona>();
                string contenido = lectura.Lee_Archivo("../lista1encrypted.txt");
                byte[] listaEncriptada1 = Convert.FromBase64String(contenido);
                List<Persona> temp1 = ayudante.Deserialize_Modelo<List<Persona>>(Decrypt(listaEncriptada1, keyAchv1));

                contenido = lectura.Lee_Archivo("../lista2encrypted.txt");
                byte[] listaEncriptada2 = Convert.FromBase64String(contenido);
                List<Persona> temp2 = ayudante.Deserialize_Modelo<List<Persona>>(Decrypt(listaEncriptada2, keyAchv2));

                contenido = lectura.Lee_Archivo("../lista3encrypted.txt");
                byte[] listaEncriptada3 = Convert.FromBase64String(contenido);
                List<Persona> temp3 = ayudante.Deserialize_Modelo<List<Persona>>(Decrypt(listaEncriptada3, keyAchv3));

                tpersonas.AddRange(temp1);
                tpersonas.AddRange(temp2);
                tpersonas.AddRange(temp3);

                return tpersonas;
            }
            else
            {
                return null;
            }
        }

        public string Decrypt(byte[] mensajeEncriptado, string psharedkey) 
        {
            try
            {
                if(psharedkey != null)
                {
                    byte[] sharedkey = Convert.FromBase64String(psharedkey);
                    string contenido = usuario.Receive(mensajeEncriptado, servidor.GetivE(), sharedkey);
                    return contenido;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }
        

        
        private void EncriptarLista(List<Persona> lista, string rutaArchivo) //obtiene una lista, y la encrypta posteriormente la guarda como un archivo encryptado
        {
            string listaAEncriptar = ayudante.Serialice_Modelo(lista);
            byte[] encryptedData;
            byte[] iv;
            byte[] encrypted = Servidor.Send(usuario.usuarioKey, listaAEncriptar, out encryptedData, out iv);
            string contenido = Convert.ToBase64String(encrypted);
            escritura.Escriba_En_TxT(contenido, "../", rutaArchivo);
        }

        
        public List<Persona> DesencriptarLista(string encryptedData, byte[] psharedKey)
        {
            byte[] iv = null;
            byte[] decryptedData = Convert.FromBase64String(encryptedData);
            string contenido = usuario.Receive(decryptedData, iv, psharedKey);
            List<Persona> listaDesencriptada = ayudante.Deserialize_Modelo<List<Persona>>(contenido);
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
        public byte[] EncriptarLista(List<Persona> lista) //obtiene una lista, la encrypta y la retorna
        {
            string listaAEncriptar = ayudante.Serialice_Modelo(lista);
            byte[] encryptedData;
            byte[] iv;
            byte [] encrypted= Servidor.Send(usuario.usuarioKey, listaAEncriptar, out encryptedData, out iv);
            return encrypted;
        }
        public List<Persona> DesencriptarLista(byte[] encryptedData, byte[] psharedKey) //obtiene una cadena de byte de una lista encryptada, los desencripta y retorna una lista
        {
            byte[] iv = null;
            string contenidoDesencriptado = usuario.Receive(encryptedData, iv, psharedKey);
            List<Persona> listaDesencriptada = ayudante.Deserialize_Modelo<List<Persona>>(contenidoDesencriptado);
            return listaDesencriptada;
        }
    }
}
