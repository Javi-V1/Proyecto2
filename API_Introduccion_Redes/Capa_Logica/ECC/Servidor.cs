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
        public byte[] servidorPublicKey;
        internal byte[] servidorKey;
        internal static byte[] ivE;
        internal static byte[] key;

        public Servidor()
        {
            using (ECDiffieHellmanCng servidor = new ECDiffieHellmanCng())
            {
                servidor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                servidor.HashAlgorithm = CngAlgorithm.Sha256;
                servidorPublicKey = servidor.PublicKey.ToByteArray();
                servidorKey = servidor.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
            }
        }

        internal static byte[] Send(byte[] pkey, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;
                aes.Key = pkey;
                aes.GenerateIV();
                iv = aes.IV;
                ivE = iv;
                key = pkey;
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

        public byte[] GetivE()
        {
            return ivE;
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
