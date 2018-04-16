﻿using Newtonsoft.Json;

namespace VE.Web.Models
{
    public class JoinRequest
    {
        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("size")]
        public int Height { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("files")]
        public string[] Files { get; set; }
    }
}