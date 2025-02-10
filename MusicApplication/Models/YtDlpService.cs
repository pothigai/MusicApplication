using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;

namespace MusicApplication.Services
{
    public class YtDlpService
    {
        private readonly YoutubeDL youtubeDL;
        private static string _dir;
        private static string _bin;

        public YtDlpService()
        {

            if (IsWindows())
            {
                _bin = "yt-dlp.exe";
            }
            else if (IsAndroid())
            {
                _bin = "yt-dlp";
            }
            youtubeDL = new YoutubeDL
            {
                YoutubeDLPath = Path.Combine(_dir, _bin) 
            };


        }

        public static async Task Init()
        {
            _dir = FileSystem.AppDataDirectory;
            await YoutubeDLSharp.Utils.DownloadYtDlp(_dir);
            await YoutubeDLSharp.Utils.DownloadFFmpeg(_dir);
        }

        public async Task<string> GetAudioStreamUrlAsync(string videoUrl)
        {
            try
            {
                var result = await youtubeDL.RunVideoDataFetch(videoUrl);

                if (result.Success && result.Data.Formats != null)
                {

                    var audioFormat = result.Data.Formats
                        .Where(format => format.AudioCodec != "none" && format.VideoCodec == "none")
                        .OrderByDescending(format => format.AudioBitrate)
                        .FirstOrDefault();

                    if (audioFormat != null)
                    {
                        Debug.WriteLine($"Extracted Audio URL: {audioFormat.Url}");
                        return audioFormat.Url;
                    }
                }

                Debug.WriteLine("No valid audio format found.");
                throw new Exception("Failed to extract audio stream URL.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetAudioStreamUrlAsync: {ex.Message}");
                throw new Exception($"Failed to fetch video data: {ex.Message}", ex);
            }
        }

        private static bool IsWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        private static bool IsAndroid()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
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