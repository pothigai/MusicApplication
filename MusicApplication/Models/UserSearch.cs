using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace MusicApplication
{
    public class UserSearch
    {

        public async Task<string> DisplayResults(string query)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "APIKEY",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 50;

            try
            {
                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<string> videos = new List<string>();
                List<string> channels = new List<string>();
                List<string> playlists = new List<string>();

                Console.WriteLine("Search Results:");
                int index = 1;

                foreach (var searchResult in searchListResponse.Items)
                {
                    string resultText = $"{index}. {searchResult.Snippet.Title}";
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            resultText += $" (Video)";
                            videos.Add(searchResult.Id.VideoId);
                            break;
                        case "youtube#channel":
                            resultText += $" (Channel)";
                            channels.Add(searchResult.Id.ChannelId);
                            break;
                        case "youtube#playlist":
                            resultText += $" (Playlist)";
                            playlists.Add(searchResult.Id.PlaylistId);
                            break;
                    }

                    Console.WriteLine(resultText);
                    index++;
                }

                if (index == 1)
                {
                    Console.WriteLine("No results found.");
                    return null;
                }

                Console.Write("\nEnter the number of the item you want to select: ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex < index)
                {
                    string selectedUrl = "";

                    if (selectedIndex <= videos.Count)
                    {
                        selectedUrl = $"https://www.youtube.com/watch?v={videos[selectedIndex - 1]}";
                    }
                    else if (selectedIndex <= videos.Count + channels.Count)
                    {
                        selectedUrl = $"https://www.youtube.com/channel/{channels[selectedIndex - 1 - videos.Count]}";
                    }
                    else if (selectedIndex <= videos.Count + channels.Count + playlists.Count)
                    {
                        selectedUrl = $"https://www.youtube.com/playlist?list={playlists[selectedIndex - 1 - videos.Count - channels.Count]}";
                    }

                    return selectedUrl;
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return null;
            }
        }
    }
}
