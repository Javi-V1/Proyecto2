using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Capa_Logica.Ayudante
{
    public class Ayudante_Encrypt
    {
        public byte[] servidorPublicKey;
        private byte[] servidorKey;

        public byte[] usuarioPublicKey;
        public Ayudante_Encrypt()
        {
            using (ECDiffieHellmanCng servidor = new ECDiffieHellmanCng())
            {

                servidor.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                servidor.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
                servidorPublicKey = servidor.PublicKey.ToByteArray();
                servidorKey = servidor.DeriveKeyMaterial(CngKey.Import(Alice.alicePublicKey, CngKeyBlobFormat.EccPublicBlob))
            }

            using (ECDiffieHellmanCng usuario = new ECDiffieHellmanCng())
            {
                usuario.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                usuario.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP256;
                usuarioPublicKey = usuario.PublicKey.ToByteArray();
            }
        }

    }
}
