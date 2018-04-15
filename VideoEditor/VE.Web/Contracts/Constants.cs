using System.Collections.Generic;

namespace VE.Web.Contracts
{
    public class Constants
    {
        public static readonly HashSet<string> VideoFormatsExtensions = new HashSet<string>(new[] { ".avi", ".webm", ".mpeg", ".mp4", ".flv", ".ogv" });
    }
}
