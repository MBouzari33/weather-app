using System.Text.Json.Serialization;

namespace WeatherApp.Models
{
    public class Clouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }
}