namespace MusicApplication.Models
{
    public class TrackInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
        public string Url { get; set; }
        public DateOnly Date { get; set; }
        public string UniqueId { get; set; }
        public string Thumbnail { get; set; }
        public string FilePath { get; set; }
    }
}
