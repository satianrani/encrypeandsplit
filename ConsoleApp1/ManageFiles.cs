using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    public class ManageFiles
    {
        private static List<string> ReadFileTemp(string folderFiles)
        {
            List<string> files = Directory.GetFiles(folderFiles).ToList<string>();
            return files;
        }

        public static void WriteSplitAndEncryptFiles(string inputFilePath, string folderSavePath, string keyAndIVBase64, int numberOfFiles)
        {
            using (var fs = File.OpenRead(inputFilePath))
            {
              //  int numberOfFiles = 10; // file should be size
                int sizeOfEachFile = (int)Math.Ceiling((double)fs.Length / numberOfFiles);
                if (Directory.Exists(folderSavePath))
                {
                    throw new Exception("Folder " + folderSavePath + " is exists");
                }
                Directory.CreateDirectory(folderSavePath);
                for (int i = 1; i <= numberOfFiles; i++)
                {
                    string baseFileName = Path.GetFileNameWithoutExtension(inputFilePath);
                    string extension = Path.GetExtension(inputFilePath);
                    //Path.GetDirectoryName(folder)
                    FileStream outputFile = new FileStream(folderSavePath + "\\" + baseFileName + "." + i.ToString().PadLeft(5, Convert.ToChar("0")) + extension + ".tmp", FileMode.Create, FileAccess.Write);
                    byte[] buffer = new byte[sizeOfEachFile];
                    int bytesRead = fs.Read(buffer, 0, sizeOfEachFile);
                    if (bytesRead > 0)
                    {
                        byte[] encodeBytes = ManangeCrypto.Encrypt(buffer, keyAndIVBase64, 256);
                        outputFile.Write(encodeBytes, 0, encodeBytes.Length);
                    }
                    outputFile.Close();
                }
            }
        }

        private static void MergeFiles(List<string> inputFilePath, string outputFilePath, string keyAndIVBase64)
        {
            //merge
            //  const int chunkSize = 20 * 1024; // 2KB // sizeOfEachFile
            //Path.GetDirectoryName(folder)
            using (var output = File.Create(outputFilePath)) //folder + "\\" + "output.pdf"))
            {
                foreach (var file in inputFilePath)
                {
                    //Path.GetDirectoryName(folder)
                    using (var input = File.OpenRead(file))
                    {
                        var buffer = new byte[input.Length];
                        int bytesRead;
                        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            byte[] decodeBytes = ManangeCrypto.Decrypt(buffer, keyAndIVBase64, 256);
                            output.Write(decodeBytes, 0, decodeBytes.Length);
                        }
                    }
                }
            }
        }

        private static string FindExtentionFile(List<string> files)
        {
            string extension = string.Empty;
            if (files.Any())
            {
                if (string.IsNullOrEmpty(extension))
                {
                    extension = Path.GetFileName(files[0]).Split('.').Reverse().ToList().ElementAt(1);
                }
            }
            return extension;
        }

        public static string DecryptAndWriteFile(string folderEncryptFiles, string keyAndIVBase64, string mergeFileNamePath)
        {
            if (!Directory.Exists(folderEncryptFiles))
                throw new Exception("No directory name "+ folderEncryptFiles+".");
            List<string> files = ManageFiles.ReadFileTemp(folderEncryptFiles);
            if (!files.Any())
                throw new Exception("No file for decrypt.");
            string extension = FindExtentionFile(files);
            string mergeFileName = string.Empty;
            if (!string.IsNullOrEmpty(extension))
            {
                mergeFileName = mergeFileNamePath + "\\" + Guid.NewGuid().ToString() + "." + extension;
                MergeFiles(files, mergeFileName, keyAndIVBase64);
            }
            return mergeFileName;
        }
    }
}