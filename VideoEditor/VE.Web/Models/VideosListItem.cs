using Newtonsoft.Json;

namespace VE.Web.Models
{
    public class VideosListItem
    {
        public VideosListItem(string name, int size)
        {
            Name = name;
            Size = size;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }
    }
}
