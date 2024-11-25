using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicApplication.Services
{
    public class YtDlpService
    {
        private const string YtDlpPath = "/data/data/com.termux/files/usr/bin/yt-dlp"; //Alternate path : C:\\yt-dlp\\yt-dlp.exe /data/data/com.termux/files/usr/bin/yt-dlp

        public async Task<string> GetAudioStreamUrlAsync(string videoUrl)
        {
            string arguments = $"-f bestaudio --get-url {videoUrl}";
            string output = string.Empty;
            string errorOutput = string.Empty;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = YtDlpPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                using (var outputReader = process.StandardOutput)
                using (var errorReader = process.StandardError)
                {
                    output = await outputReader.ReadToEndAsync();
                    errorOutput = await errorReader.ReadToEndAsync();
                }

                process.WaitForExit();

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    Console.WriteLine($"Error from yt-dlp: {errorOutput}");
                }

                if (string.IsNullOrEmpty(output.Trim()))
                {
                    throw new Exception("No valid stream URL found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAudioStreamUrlAsync: {ex.Message}");
                throw; 
            }

            return output.Trim();
        }
    }
}
