using Newtonsoft.Json;
using System.Collections.Generic;

namespace Flick.Controllers.Classes
{
    public class Flickr
    {
        [JsonProperty("items")]
        public List<FlickrItem> Items { get; set; }
    }

    public class FlickrItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("media")]
        public Dictionary<string, string> Media { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}
