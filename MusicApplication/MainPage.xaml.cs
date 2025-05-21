using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MusicApplication.Models;
using MediaManager;

namespace MusicApplication
{
    public partial class MainPage : ContentPage
    {
        private readonly UserSearch _userSearch;
        private readonly ObservableCollection<TrackInfo> _searchResults;
        private TrackInfo _selectedTrack;

        private readonly string _serverUrl = "http://192.168.2.8:8080/";

        public MainPage()
        {
            InitializeComponent();
            _userSearch = new UserSearch();
            _searchResults = new ObservableCollection<TrackInfo>();
            ResultsCollectionView.ItemsSource = _searchResults;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            string keyword = SearchEntry.Text;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                ResultLabel.Text = "Searching...";
                _searchResults.Clear();

                try
                {
                    var results = await _userSearch.DisplayResults(keyword);

                    if (results != null && results.Count > 0)
                    {
                        foreach (var result in results)
                        {
                            _searchResults.Add(result);
                        }
                        ResultLabel.Text = "Select a track from the list";
                    }
                    else
                    {
                        ResultLabel.Text = "No results found";
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while searching: {ex.Message}", "OK");
                }
            }
        }

        private async void OnTrackSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                _selectedTrack = (TrackInfo)e.CurrentSelection[0];
                MetadataLabel.Text = $"Title: {_selectedTrack.Title}\nArtist: {_selectedTrack.Artist}\nDuration: {_selectedTrack.Duration}";

                try
                {
                    string videoId = GetVideoIdFromUrl(_selectedTrack.Url);
                    string streamUrl = await GetStreamUrlFromServer(videoId);

                    if (!string.IsNullOrEmpty(streamUrl))
                    {
                        Debug.WriteLine($"Stream URL: {streamUrl}");
                        await CrossMediaManager.Current.Play(streamUrl);
                    }
                    else
                    {
                        await DisplayAlert("Error", "No stream URL received from server.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to get stream URL: {ex.Message}", "OK");
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private string GetVideoIdFromUrl(string url)
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"];
        }

        private async Task<string> GetStreamUrlFromServer(string videoId)
        {
            using var client = new HttpClient();
            var payload = new { videoId = videoId };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_serverUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ServerResponse>(json);
                return result?.url;
            }
            return null;
        }

        private async void OnPlayClicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMediaManager.Current.Play();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error while playing: {ex.Message}", "OK");
            }
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            try
            {
                CrossMediaManager.Current.Pause();
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error while pausing: {ex.Message}", "OK");
            }
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            try
            {
                CrossMediaManager.Current.Stop();
                MetadataLabel.Text = "";
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error while stopping: {ex.Message}", "OK");
            }
        }

        private class ServerResponse
        {
            public string url { get; set; }
        }
    }
}
