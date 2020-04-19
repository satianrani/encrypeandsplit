using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ManageCrtptoEtc
    {
        public static ICryptoTransform AlgorithmTripleDES()
        {
            ICryptoTransform alg = null;
            using (TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider())
            {
                byte[] Key = { 0x9b, 0x67, 0x14, 0xc, 0xb8, 0x7e, 0xf0, 0x4b, 0x6e, 0xd, 0x88, 0x7a, 0xf1, 0xbb, 0x33, 0xc1, 0xc1, 0x12, 0xa3, 0x1f, 0xca, 0x2d, 0xdc, 0x54 };
                byte[] IV = { 0x3d, 0x12, 0xe9, 0x8c, 0xea, 0x24, 0x61, 0xf0 };
                alg = TDES.CreateEncryptor(Key, IV);
            }
            return alg;
        }

        public static ICryptoTransform AlgorithmAES()
        {
            ICryptoTransform alg = null;
            byte[] IV = Encoding.UTF8.GetBytes("125sdfsdfsdfsdfhhgkjklkjhertyuio");  // CRB mode uses an empty IV
            byte[] Key = Encoding.UTF8.GetBytes("12dsdfsdfsdfsdfhhgkjklkjhertyuio");  // Byte array representing the key
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.BlockSize = 256;
                aesAlg.Padding = PaddingMode.None;
                alg = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            }
            return alg;
        }

        public static byte[] EncryptStringToBytes(byte[] data, ICryptoTransform alg)
        {
            byte[] encrypted;

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, alg, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(data, 0, data.Length);
                    csEncrypt.FlushFinalBlock();
                    //   return msEncrypt.ToArray();
                    encrypted = new byte[msEncrypt.Length];
                    msEncrypt.Position = 0;
                    msEncrypt.Read(encrypted, 0, data.Length);
                }
            }
            return encrypted;
        }

        public static byte[] DecryptStringFromBytes(byte[] cipherBytes, ICryptoTransform alg)
        {
            byte[] decrypted;
            // Create the streams used for decryption.
            using (MemoryStream output = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(output, alg, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(cipherBytes, 0, cipherBytes.Length);
                    csDecrypt.FlushFinalBlock();
                    decrypted = output.ToArray();
                }
            }
            return decrypted;
        }

        public static byte[] DecryptStringFromBytes2(byte[] cipherBytes, ICryptoTransform alg)
        {
            using (var encryptedStream = new MemoryStream(cipherBytes))
            {
                //stream where decrypted contents will be stored
                using (var decryptedStream = new MemoryStream())
                {
                    //decrypt stream and write it to parent stream
                    using (var cryptoStream = new CryptoStream(encryptedStream, alg, CryptoStreamMode.Read))
                    {
                        int data;

                        while ((data = cryptoStream.ReadByte()) != -1)
                            decryptedStream.WriteByte((byte)data);
                    }

                    decryptedStream.Position = 0;
                    return decryptedStream.ToArray();
                }
            }
        }

        public static byte[] AesEncryptor(byte[] key, byte[] iv, byte[] payload)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;

                aesAlg.Padding = PaddingMode.PKCS7;

                var encryptor = aesAlg.CreateEncryptor(key, iv);

                var encrypted = encryptor.TransformFinalBlock(payload, 0, payload.Length);

                return iv.Concat(encrypted).ToArray();
            }
        }

        public static byte[] AesDecryptor(byte[] key, byte[] iv, byte[] payload)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;

                aesAlg.Padding = PaddingMode.PKCS7;

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                return decryptor.TransformFinalBlock(payload, 0, payload.Length);
            }
        }
    }
}
