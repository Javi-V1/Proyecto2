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
        private ECDiffieHellmanCng usuario;
        public byte[] usuarioPublicKey;
        public byte[] usuarioKey;
        
        public Usuario(byte[] serverPublicKey)
        {
            usuario = new ECDiffieHellmanCng();

            usuario.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            usuario.HashAlgorithm = CngAlgorithm.Sha256;   

            usuarioPublicKey = usuario.PublicKey.ToByteArray();

            CngKey serverKey = CngKey.Import(serverPublicKey, CngKeyBlobFormat.EccPublicBlob);
            usuario.KeySize = 256;
            byte[] sharedSecret = usuario.DeriveKeyMaterial(serverKey);
            usuarioKey = sharedSecret;
        }
        public byte[] GetSharedKey(Usuario usuario)
        {
            return usuarioKey;
        }

        public string Receive(byte[] encryptedMessage, byte[] iv, byte[] claveCifrado)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;

                aes.Key = claveCifrado;
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
