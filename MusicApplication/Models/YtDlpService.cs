using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicApplication.Services
{
    public class YtDlpService
    {
        private readonly HttpClient _httpClient;

        public YtDlpService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetAudioStreamUrlAsync(string videoUrl)
        {
            try
            {
               
                var uri = new Uri(videoUrl);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var videoId = query["v"];

                if (string.IsNullOrEmpty(videoId))
                    throw new Exception("Invalid YouTube URL");
              
                string serverUrl = "http://192.168.2.8:8080/"; 
                var json = JsonSerializer.Serialize(new { videoId });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(serverUrl, content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();               
                using var doc = JsonDocument.Parse(responseString);

                if (doc.RootElement.TryGetProperty("url", out var urlElement))
                {
                    return urlElement.GetString();
                }

                throw new Exception("URL not found in response.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stream URL: {ex.Message}");
                return null;
            }
        }
    }
}
