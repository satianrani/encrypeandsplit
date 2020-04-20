using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //string keyAndIVBase64 = ManangeCrypto.GenerateKey(256);
            string inputFilePath = @"C:\Users\cmi\Desktop\encrypeandsplit-master\ConsoleApp1\test.pdf";
            string currentDirectoryFile = Path.GetDirectoryName(inputFilePath);
            string newFolderName = Guid.NewGuid().ToString();
            string saveSplitFolderPath = currentDirectoryFile + "\\" + newFolderName;
            string keyAndIVBase64 = "WTBES2ZLU2owSW5SbEh1S044QkpwaUozRE5zTzVFOVFWUlJCNkJnUGpLMD0sQTZuN2J6cnlvV09INzk1bTlnTU9vcGJVenduOWFkMEYyQzJSclIySm1FYz0=";
            int numberOfFiles = 10;
            ManageFiles.WriteSplitAndEncryptFiles(inputFilePath, saveSplitFolderPath, keyAndIVBase64, numberOfFiles);


            string mergeFileName = currentDirectoryFile;
            string folderEncryptFiles = currentDirectoryFile + "\\" + newFolderName;

            string saveFilePath = ManageFiles.DecryptAndWriteFile(folderEncryptFiles, keyAndIVBase64, mergeFileName);
            Console.WriteLine(saveFilePath);
            Console.ReadLine();
        }

    }
}