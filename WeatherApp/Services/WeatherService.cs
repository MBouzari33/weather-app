using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;


        private readonly string _apiKey;


        private const string BaseUrl =
            "https://api.openweathermap.org/data/2.5/weather";

        public WeatherService()
        {
            _apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("API Key not configured.");
            _httpClient = new HttpClient();
        }

        public async Task<WeatherResponse> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City must not be empty.");

            var url =
                $"{BaseUrl}?q={city}&appid={_apiKey}&units=metric";

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (HttpRequestException)
            {
                throw new Exception("No internet connection.");
            }

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 404)
                    throw new Exception("City not found.");

                throw new Exception(
                    $"API Error: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<WeatherResponse>(
                    json, options);

                if (result == null)
                    throw new Exception("Invalid API response.");

                return result;
            }
            catch (JsonException)
            {
                throw new Exception("Failed to parse weather data.");
            }
        }
    }
}