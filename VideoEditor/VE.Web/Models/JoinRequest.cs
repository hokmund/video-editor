using Newtonsoft.Json;

namespace VE.Web.Models
{
    public class JoinRequest
    {
        [JsonProperty("files")]
        public string[] Files { get; set; }
    }
}
