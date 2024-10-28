using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicApplication.Models;

namespace MusicApplication.Models
{
    public class PlayerStatus
    {
        public enum Status
        {
            playing,
            paused,
            stopped,
            resume
        }
        public TrackInfo Track { get; set; }
        public TimeSpan PlaybackTime { get; set; }
        public Status AudioStatus { get; set; }
        public PlayerStatus(TrackInfo track, Status audioStatus)
        {
            Track = track;
            AudioStatus = audioStatus;
            PlaybackTime = TimeSpan.Zero;
        }
    }
}
