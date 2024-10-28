using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApplication
{
    public class DownloadedAudio
    {
        public enum AudioFormat
        {
            MP3,
            WAV,
            MP4A
        }

        public AudioFormat Format { get; set; }
        public string AudioUrl { get; set; }
    }
}
