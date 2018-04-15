using System.Collections.Generic;
using System.IO;
using VE.Web.Models;

namespace VE.Web.Services
{
    public static class TempFilesUtils
    {
        public static void CleanTempFiles(IEnumerable<string> tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        public static string GetMediaFile(string fileNameTemplate, params object[] inputs)
        {
            return Path.Combine(Constants.MediaDataFolder, string.Format(fileNameTemplate, inputs));
        }

        public static string GetTempFile(string fileNameTemplate, params object[] inputs)
        {
            return Path.Combine(Constants.TempDataFolder, string.Format(fileNameTemplate, inputs));
        }
    }
}
