using System.Collections.Generic;
using System.IO;

namespace VE.Web.Services
{
    public static class FilesUtils
    {
        public const string AppDataFolder = "AppData";

        public static readonly string MediaDataFolder = Path.Combine(AppDataFolder, "Media");
        public static readonly string TempDataFolder = Path.Combine(AppDataFolder, "Temp");

        public static void CleanTempFiles(IEnumerable<string> tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        public static string GetMediaFile(string fileNameTemplate, params object[] inputs)
        {
            return Path.Combine(MediaDataFolder, string.Format(fileNameTemplate, inputs));
        }

        public static string GetTempFile(string fileName)
        {
            return Path.Combine(TempDataFolder, fileName);
        }
    }
}
