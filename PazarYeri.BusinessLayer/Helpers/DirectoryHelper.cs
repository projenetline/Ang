using System.IO;
using System.Reflection;

namespace PazarYeri.BusinessLayer.Helpers
{
    public static class DirectoryHelper
    {
        public static void CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetAppDirectoryPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
