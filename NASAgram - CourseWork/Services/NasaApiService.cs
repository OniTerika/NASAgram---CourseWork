using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using NasaApp.Models;

namespace NasaApp.Services
{
    public class NasaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "Nasa API Key";

        public NasaApiService()
        {
            _httpClient = new HttpClient();
        }

        // APOD
        public async Task<ApodModel> GetPhotoOfTheDayAsync()
        {
            string url = $"https://api.nasa.gov/planetary/apod?api_key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApodModel>(json);
        }

        // RANDOM
        public async Task<ApodModel> GetRandomPhotoAsync()
        {
            // Параметр count=1 - повернути масив з 1 випадкового фото
            string url = $"https://api.nasa.gov/planetary/apod?api_key={_apiKey}&count=1";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            string json = await response.Content.ReadAsStringAsync();
            // Повертає список, беремо [0]
            var list = JsonConvert.DeserializeObject<List<ApodModel>>(json);
            return list != null && list.Count > 0 ? list[0] : null;
        }

        // ASTERMOUNT
        public async Task<AsteroidModel> GetAsteroidsTodayAsync()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string url = $"https://api.nasa.gov/neo/rest/v1/feed?start_date={today}&end_date={today}&api_key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AsteroidModel>(json);
        }
    }
}