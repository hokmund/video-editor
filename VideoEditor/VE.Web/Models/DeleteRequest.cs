using Newtonsoft.Json;

namespace VE.Web.Models
{
  public class DeleteRequest
  {
    [JsonProperty("fileId")]
    public string FileId { get; set; }

    [JsonProperty("type")]
    public MediaType Type { get; set; }
  }
}
