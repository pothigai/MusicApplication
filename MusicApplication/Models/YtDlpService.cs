using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;

namespace MusicApplication.Services
{
    public class YtDlpService
    {
        private readonly YoutubeDL youtubeDL;

        public YtDlpService()
        {
            string ytDlpPath = GetYtDlpPath();
            youtubeDL = new YoutubeDL
            {
                YoutubeDLPath = ytDlpPath
            };
        }

        public async Task<string> GetAudioStreamUrlAsync(string videoUrl)
        {
            try
            {
                var result = await youtubeDL.RunVideoDataFetch(videoUrl);

                if (result.Success && !string.IsNullOrEmpty(result.Data?.Url))
                {
                    Console.WriteLine($"Extracted URL: {result.Data.Url}");
                    return result.Data.Url;
                }
                else
                {
                    Console.WriteLine("yt-dlp output: " + string.Join("\n", result.ErrorOutput));
                    throw new Exception("Failed to extract audio stream URL.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAudioStreamUrlAsync: {ex.Message}");
                throw new Exception($"Failed to fetch video data: {ex.Message}", ex);
            }
        }

        private string GetYtDlpPath()
        {
            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");

            if (!File.Exists(ytDlpPath))
            {
                throw new FileNotFoundException($"yt-dlp binary not found at {ytDlpPath}. Ensure the binary is correctly placed.");
            }

            return ytDlpPath;
        }

    }
}
