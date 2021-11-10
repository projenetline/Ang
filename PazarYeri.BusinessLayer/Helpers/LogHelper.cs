using System;
using System.IO;

namespace PazarYeri.BusinessLayer.Helpers
{
    public class LogHelper
    {
        private readonly string _applicationPath = DirectoryHelper.GetAppDirectoryPath();

        public LogHelper(string fileName = "", string message = "", string subFolder = "")
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = $"{DateTime.Today:yyyyMMdd}";

            var folderName = $"logs\\{DateTime.Today:yyyyMM}";

            DirectoryHelper.CreateFolderIfNotExists(folderName);

            if (!string.IsNullOrEmpty(subFolder))
                folderName = folderName + "\\" + subFolder;

            DirectoryHelper.CreateFolderIfNotExists(folderName);

            try
            {
                File.WriteAllText($@"{_applicationPath}\{folderName}\{fileName}.txt", "");

                using (StreamWriter streamWriter = File.AppendText($@"{_applicationPath}\{folderName}\{fileName}.txt"))
                {
                    streamWriter.WriteLine($"{message}");
                }
            }
            catch (Exception)
            {
            }
        }
    }
}