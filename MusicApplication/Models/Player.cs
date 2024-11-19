using MediaManager;
using MusicApplication.Models;
using MusicApplication.Services;
using System.Threading.Tasks;

namespace MusicApplication
{
    public class Player
    {
        private readonly YtDlpService _ytDlpService = new YtDlpService();
        private TrackInfo _currentTrack;

        public async Task PlayAsync(TrackInfo track)
        {
            if (track == null) return;

            _currentTrack = track;

            string audioStreamUrl = await _ytDlpService.GetAudioStreamUrlAsync(track.Url);

            if (string.IsNullOrEmpty(audioStreamUrl))
            {
                Console.WriteLine("Failed to fetch audio stream URL.");
                return;
            }
            await CrossMediaManager.Current.Play(audioStreamUrl);
        }

        public void Pause()
        {
            CrossMediaManager.Current.Pause();
        }

        public void Stop()
        {
            CrossMediaManager.Current.Stop();
        }
    }
}
