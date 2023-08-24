using Capa_Acceso_Datos.Txt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica.ECC
{
    public class Servidor
    {
        private ECDiffieHellmanCng servidor;
        public byte[] servidorPublicKey;
        internal byte[] servidorKey;
        internal byte[] key;
        internal List<byte[]> claves = new List<byte[]>();
        public Escritura_Txt escritura = new Escritura_Txt();

        public Servidor()
        {
            servidor = new ECDiffieHellmanCng();
            servidor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            servidor.HashAlgorithm = CngAlgorithm.Sha256;
            servidor.KeySize = 256;
            servidorPublicKey = servidor.PublicKey.ToByteArray();
            servidorKey = servidor.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
        }

        internal byte[] EncryptMessage(string secretMessage,byte[] usuarioPublickey, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;
                aes.GenerateIV();
                iv = aes.IV;
                Servidor servidor = new Servidor();
                byte[] claveCifrado = servidor.DeriveKeyMaterial(usuarioPublickey);
                claves.Add(claveCifrado);
                aes.Key = claveCifrado;
                //Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    return ciphertext.ToArray();
                }
            }
        }
        public void GuardarClave()
        {
            List<string> guardar = new List<string> (); 
            foreach (byte[] clave in claves)
            {
                string claveStr = Convert.ToBase64String(clave);
                guardar.Add(claveStr);
            }
            string clavesTexto = string.Join(Environment.NewLine, guardar);
            escritura.Escriba_En_TxT(clavesTexto, "../", "Claves.txt");
        }

        internal byte[] DeriveKeyMaterial(byte[] usuarioPublicKey)
        {
            using (ECDiffieHellmanCng servidor = new ECDiffieHellmanCng())
            {
                CngKey usuarioKey = CngKey.Import(usuarioPublicKey, CngKeyBlobFormat.EccPublicBlob);
                servidor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                servidor.HashAlgorithm = CngAlgorithm.Sha256;
                return servidor.DeriveKeyMaterial(usuarioKey);
            }
        }
    }
}
