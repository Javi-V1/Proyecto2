using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica.ECC
{
    public class Servidor
    {
        public byte[] servidorPublicKey;
        internal byte[] servidorKey;

        public Servidor()
        {
            using (ECDiffieHellmanCng servidor = new ECDiffieHellmanCng())
            {
                servidor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                servidor.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
                servidorPublicKey = servidor.PublicKey.ToByteArray();
            }
        }

        internal static byte[] Send(byte[] key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;

                //Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    encryptedMessage = ciphertext.ToArray();
                    return encryptedMessage;
                }
            }
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
