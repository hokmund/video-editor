using System.Collections.Generic;
using System.IO;

namespace VE.Web.Services
{
    public static class TempFilesUtils
    {
        private const string AppDataFolder = "AppData";

        public static void CleanTempFiles(IEnumerable<string> tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        public static string GetTempFile(string fileNameTemplate, params object[] inputs)
        {
            return Path.Combine(AppDataFolder, string.Format(fileNameTemplate, inputs));
        }
    }
}
