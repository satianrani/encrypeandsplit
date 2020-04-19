using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    public class ManageFiles
    {
        byte[] Key = { 0x9b, 0x67, 0x14, 0xc, 0xb8, 0x7e, 0xf0, 0x4b, 0x6e, 0xd, 0x88, 0x7a, 0xf1, 0xbb, 0x33, 0xc1, 0xc1, 0x12, 0xa3, 0x1f, 0xca, 0x2d, 0xdc, 0x54 };
        byte[] IV = { 0x3d, 0x12, 0xe9, 0x8c, 0xea, 0x24, 0x61, 0xf0 };
        public static string genBase64 = "QXFqUlVIMlk5VjFYc2Qyc050NlMzUT09LFNCZkZlUDRYeTAyNGpFdXNJK2dZbXI2ZzBhOHVJMy9xcXJNZXVKRU9hWU09";
        public static List<string> ReadFileTemp(string folderFiles)
        {
            var a = Directory.GetFiles(folderFiles).ToList<string>();
            return a;
        }
      
        public static void splitFile(string inputFile, string folderSave)
        {
          //  var gen = GenerateKey(256);
           // var IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(gen)).Split(',')[0]);
           // var Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(gen)).Split(',')[1]);

            var fs = File.OpenRead(inputFile);
            // Console.WriteLine(fs.Length);
            // cut
            int numberOfFiles = 10; // file should be size
            int sizeOfEachFile = (int)Math.Ceiling((double)fs.Length / numberOfFiles);
            ICryptoTransform alg = ManangeCrypto.AlgorithmTripleDES();
            for (int i = 1; i <= numberOfFiles; i++)
            {
                string baseFileName = Path.GetFileNameWithoutExtension(inputFile);
                string extension = Path.GetExtension(inputFile);
                //Path.GetDirectoryName(folder)
                FileStream outputFile = new FileStream(folderSave + "\\" + baseFileName + "." + i.ToString().PadLeft(5, Convert.ToChar("0")) + extension + ".tmp", FileMode.Create, FileAccess.Write);
                 
                byte[] buffer = new byte[sizeOfEachFile];
                int bytesRead = fs.Read(buffer, 0, sizeOfEachFile);
                if (bytesRead > 0)
                { 
                    byte[] encodeBytes = ManangeCrypto.Encrypt(buffer, genBase64, 256); 
                    outputFile.Write(encodeBytes, 0, encodeBytes.Length);
                }
                outputFile.Close();
            }
            fs.Close();
            
        }

        public static void merge(List<string> inputFiles, string outputFile)
        {
            //merge
            //  const int chunkSize = 20 * 1024; // 2KB // sizeOfEachFile
            //Path.GetDirectoryName(folder)
            using (var output = File.Create(outputFile)) //folder + "\\" + "output.pdf"))
            {
                foreach (var file in inputFiles)
                {
                    //Path.GetDirectoryName(folder)
                    using (var input = File.OpenRead(file))
                    {
                        var buffer = new byte[input.Length];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            byte[] decodeBytes = ManangeCrypto.Decrypt(buffer, genBase64, 256);
                            output.Write(decodeBytes, 0, decodeBytes.Length);
                        }
                    }
                }
            }
        }
    }
}