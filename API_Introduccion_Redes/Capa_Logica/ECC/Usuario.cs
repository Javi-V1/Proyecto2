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
        private byte[] usuarioKey;
        public Usuario()
        {
            using(ECDiffieHellmanCng usuario = new ECDiffieHellmanCng())
            {
                Servidor servidor = new Servidor(); 
                usuario.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                usuario.HashAlgorithm = CngAlgorithm.Sha256;
                usuarioPublicKey = usuario.PublicKey.ToByteArray();
                usuarioKey = usuario.DeriveKeyMaterial(CngKey.Import(servidor.servidorPublicKey, CngKeyBlobFormat.EccPublicBlob))
            }
        }

        public string Receive(byte[] encryptedMessage, byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = usuarioKey;
                aes.IV = iv;

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
