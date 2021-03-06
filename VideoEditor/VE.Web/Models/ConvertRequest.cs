﻿using Newtonsoft.Json;

namespace VE.Web.Models
{
    public class ConvertRequest
    {
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }
    }
}
