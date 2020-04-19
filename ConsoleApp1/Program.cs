using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ManangeCrypto.GenerateKey(256);
            string inputFile = @"E:\DotNetWorkspace\ConsoleApp1\ConsoleApp1\cover.jpg";
            string saveSplitFolder = Path.GetDirectoryName(inputFile) + "\\" + Guid.NewGuid().ToString();
             
            ManageFiles.splitFile(inputFile, saveSplitFolder);

            //Merge
            var files = ManageFiles.ReadFileTemp(saveSplitFolder);
            string extension = string.Empty;
            if (files.Any())
            {
                if (string.IsNullOrEmpty(extension)) {
                    extension = Path.GetFileName(files[0]).Split('.').Reverse().ToList().ElementAt(1);
                } 
                string mergeFileName = Path.GetDirectoryName(inputFile) + "\\" + Guid.NewGuid().ToString() + "." + extension;
              // int chunkSize = 20 * 1024;
                ManageFiles.merge(files, mergeFileName);
            }

            Console.ReadLine();
        }
    }
}