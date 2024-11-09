using System.Diagnostics;
using System.Threading.Tasks;

namespace MusicApplication.Models
{
    public class YtDlpService
    {
        private readonly string _termuxPath = "/data/data/com.termux/files/usr/bin/sh"; 

        public async Task<string> DownloadAudioAsync(string url)
        {
            string outputTemplate = "%(title)s.%(ext)s";
            string command = $"-x --audio-format wav -o \"{outputTemplate}\" {url}";
            var result = await ExecuteTermuxCommand(command);
            return result;
        }

        public async Task<string> GetMetadataAsync(string url)
        {
            string command = $"--dump-json {url}";
            return await ExecuteTermuxCommand(command);
        }

        private async Task<string> ExecuteTermuxCommand(string ytDlpCommand)
        {
            var fullCommand = $"{_termuxPath} -c \"yt-dlp {ytDlpCommand}\"";
            var processInfo = new ProcessStartInfo
            {
                FileName = "/system/bin/sh",
                Arguments = $"-c \"{fullCommand}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processInfo })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.WriteLine($"yt-dlp error: {error}");
                }

                return output;
            }
        }
    }
}
