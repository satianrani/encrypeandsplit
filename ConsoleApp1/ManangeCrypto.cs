using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    public class ManangeCrypto
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
            byte[] IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // CRB mode uses an empty IV
            byte[] Key = Encoding.UTF8.GetBytes("dsdfsdfsdfsdfhhgkjklkjhertyuiott");  // Byte array representing the key
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
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
        public static byte[] Encrypt(byte[] plainText, string iCompleteEncodedKey, int iKeySize)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = iKeySize;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[0]);
            aesEncryption.Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[1]);
           // byte[] plainText = ASCIIEncoding.UTF8.GetBytes(iPlainStr);
            ICryptoTransform crypto = aesEncryption.CreateEncryptor();
            byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return cipherText;
          //  return Convert.ToBase64String(cipherText);
        }
        public static byte[] Decrypt(byte[] encryptedBytes, string iCompleteEncodedKey, int iKeySize)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = iKeySize;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[0]);
            aesEncryption.Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[1]);
            ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
            //byte[] encryptedBytes = Convert.FromBase64CharArray(iEncryptedText.ToCharArray(), 0, iEncryptedText.Length);
            return decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        //  return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
        }
        public static string GenerateKey(int iKeySize)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = iKeySize;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.GenerateIV();
            string ivStr = Convert.ToBase64String(aesEncryption.IV);
            aesEncryption.GenerateKey();
            string keyStr = Convert.ToBase64String(aesEncryption.Key);
            string completeKey = ivStr + "," + keyStr;

            return Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(completeKey));
        }
    }
}