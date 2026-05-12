using System.Text.Json.Serialization;

namespace WeatherApp.Models
{
    public class Wind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }

        [JsonPropertyName("gust")]
        public double? Gust { get; set; }
    }
}