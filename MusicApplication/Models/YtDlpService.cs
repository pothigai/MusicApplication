using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicApplication.Services
{
    public class YtDlpService
    {
        private const string YtDlpPath = "/data/data/com.termux/files/usr/bin/yt-dlp"; 

        public async Task<string> GetAudioStreamUrlAsync(string videoUrl)
        {
            string arguments = $"-f bestaudio --get-url {videoUrl}";
            string output = string.Empty;

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = YtDlpPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                using (var reader = process.StandardOutput)
                {
                    output = await reader.ReadToEndAsync();
                }

                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting audio URL: {ex.Message}");
            }

            return output.Trim(); 
        }
    }
}
