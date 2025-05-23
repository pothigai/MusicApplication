using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MusicApplication.Models;

namespace MusicApplication
{
    public class UserSearch
    {
        public async Task<List<TrackInfo>> DisplayResults(string query)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "API_KEY",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 10; 

            try
            {
                var searchListResponse = await searchListRequest.ExecuteAsync();
                var tracks = new List<TrackInfo>();

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        var track = new TrackInfo
                        {
                            Title = searchResult.Snippet.Title,
                            Artist = searchResult.Snippet.ChannelTitle,
                            Url = $"https://www.youtube.com/watch?v={searchResult.Id.VideoId}",
                            Duration = TimeSpan.Zero  
                        };
                        tracks.Add(track);
                    }
                }

                return tracks;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return null;
            }
        }
    }
}
