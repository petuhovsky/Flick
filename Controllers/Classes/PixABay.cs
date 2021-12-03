using Newtonsoft.Json;
using System.Collections.Generic;

namespace Flick.Controllers.Classes
{
    public class PixABay
    {
        [JsonProperty("hits")]
        public List<Hit> Hits { get; set; }
    }

    public class Hit
    {
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("previewURL")]
        public string PreviewURL { get; set; }

        [JsonProperty("pageURL")]
        public string PageURL { get; set; }
    }
}
