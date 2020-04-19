using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string inputFile = @"E:\DotNetWorkspace\ConsoleApp1\ConsoleApp1\test.pdf";
            string saveSplitFolder = Path.GetDirectoryName(inputFile) + "\\" + Guid.NewGuid().ToString();

            System.IO.Directory.CreateDirectory(saveSplitFolder);
            ManageFiles.splitFile(inputFile, saveSplitFolder);

            //Merge
            var files = ManageFiles.ReadFileTemp(saveSplitFolder);
            if (files.Any())
            {
                string extension = Path.GetFileName(files[0]).Split('.').Reverse().ToList().ElementAt(1);
                string mergeFileName = Path.GetDirectoryName(inputFile) + "\\" + Guid.NewGuid().ToString() + "." + extension;
              // int chunkSize = 20 * 1024;
                ManageFiles.merge(files, mergeFileName);
            }

            Console.ReadLine();
        }
    }
}