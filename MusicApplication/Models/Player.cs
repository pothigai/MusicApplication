using MusicApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApplication
{
    public class Player
    {
        public PlayerStatus CurrentStatus { get; set; }

        public void Play(TrackInfo track)
        {
            CurrentStatus = new PlayerStatus(track, PlayerStatus.Status.playing);
        }

        public void Pause()
        {
            if (CurrentStatus != null && CurrentStatus.AudioStatus == PlayerStatus.Status.playing)
            {
                CurrentStatus.AudioStatus = PlayerStatus.Status.paused;
            }
        }

        public void Resume()
        {
            if (CurrentStatus != null && CurrentStatus.AudioStatus == PlayerStatus.Status.paused)
            {
                CurrentStatus.AudioStatus = PlayerStatus.Status.playing;
            }
        }

        public void Stop()
        {
            if (CurrentStatus != null)
            {
                CurrentStatus.AudioStatus = PlayerStatus.Status.stopped;
            }
        }
    }
}
