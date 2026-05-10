using Newtonsoft.Json;

namespace NasaApp.Models
{
    // 1: APOD
    public class ApodModel
    {
        [JsonProperty("title")] public string Title { get; set; } = string.Empty;
        [JsonProperty("explanation")] public string Explanation { get; set; } = string.Empty;
        [JsonProperty("url")] public string Url { get; set; } = string.Empty;
        [JsonProperty("media_type")] public string MediaType { get; set; } = string.Empty;
    }

    // 2: Астероїди
    public class AsteroidModel
    {
        [JsonProperty("element_count")]
        public int ElementCount { get; set; }
    }
}