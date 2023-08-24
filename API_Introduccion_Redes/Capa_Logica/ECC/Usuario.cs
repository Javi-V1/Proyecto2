using Capa_Acceso_Datos.Txt;
using Capa_Modelo.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica.ECC
{
    public class Usuario
    {
        public byte[] usuarioPublicKey;
        internal byte[] usuarioKey;
        internal byte[] sharedkey;
        public Usuario(byte[] serverPublicKey)
        {
            using(ECDiffieHellmanCng usuario = new ECDiffieHellmanCng())
            {
                Servidor servidor = new Servidor(); 
                usuario.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                usuario.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
                CngKey serverKey = CngKey.Import(serverPublicKey, CngKeyBlobFormat.EccPublicBlob);
                this.usuarioPublicKey = usuario.PublicKey.ToByteArray();
                this.usuarioKey = usuario.DeriveKeyMaterial(serverKey);
                this.sharedkey = this.usuarioKey;
            }
        }
        public byte[] GetSharedKey()
        {
            return sharedkey;
        }

        public string Receive(byte[] encryptedMessage, byte[] piv, byte[] psharedKey)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = psharedKey;
                aes.IV = piv;
                //Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        return message;
                    }
                }
                
            }
        }

    }
}
