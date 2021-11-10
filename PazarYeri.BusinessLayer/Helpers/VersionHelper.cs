using System.Diagnostics;

namespace PazarYeri.BusinessLayer.Helpers
{
    public static class VersionHelper
    {
        public static string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }
    }
}