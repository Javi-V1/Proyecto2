using Capa_Acceso_Datos.Txt;
using Capa_Modelo.Persona;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica.ECC
{
    public class Usuario
    {
        public byte[] usuarioPublicKey;
        public byte[] usuarioKey;
        
        public Usuario(byte[] serverPublicKey)
        {
            using(ECDiffieHellmanCng usuario = new ECDiffieHellmanCng())
            {
                
                Servidor servidor = new Servidor(); 
                usuario.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                usuario.HashAlgorithm = CngAlgorithm.Sha256;
                CngKey serverKey = CngKey.Import(serverPublicKey, CngKeyBlobFormat.EccPublicBlob);
                this.usuarioPublicKey = usuario.PublicKey.ToByteArray();
                this.usuarioKey = usuario.DeriveKeyMaterial(serverKey);
            }
        }
        public byte[] GetSharedKey()
        {
            return usuarioKey;
        }

        public string Receive(byte[] encryptedMessage, byte[] piv, byte[] psharedKey)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;
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
