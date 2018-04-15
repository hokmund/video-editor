using System.IO;

namespace VE.Web.Models
{
    public class Constants
    {
        public const string AppDataFolder = "AppData";

        public static readonly string MediaDataFolder = Path.Combine(AppDataFolder, "Media");

        public static readonly string TempDataFolder = Path.Combine(AppDataFolder, "Temp");
    }
}
