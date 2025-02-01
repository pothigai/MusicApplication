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

        public YtDlpService()
        {
            //    string ytDlpPath = GetYtDlpPath();
            youtubeDL = new YoutubeDL
            {
                YoutubeDLPath = Path.Combine(_dir, "yt-dlp.exe") //TODO: needs to be customized for vaious OSs, see implementation in Utils https://github.com/Bluegrams/YoutubeDLSharp/blob/b2f7968a2ef06a9c7b2c212785cfeac0b187b6d8/YoutubeDLSharp/Utils.cs#L146
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