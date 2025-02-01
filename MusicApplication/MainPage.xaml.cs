using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MusicApplication.Models;
using MediaManager;
using MusicApplication.Services;
using System.Diagnostics;


namespace MusicApplication
{
    public partial class MainPage : ContentPage
    {
        private readonly UserSearch _userSearch;
        private readonly ObservableCollection<TrackInfo> _searchResults;
        private TrackInfo _selectedTrack;

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
                    await YtDlpService.Init();
                    var ytdlp = new YtDlpService();
                    var streamUrl = await ytdlp.GetAudioStreamUrlAsync(_selectedTrack.Url);
                    Debug.WriteLine(streamUrl);


                    if (!string.IsNullOrEmpty(streamUrl))
                    {
                        Console.WriteLine($"Stream URL: {streamUrl}");

                        await CrossMediaManager.Current.Play(streamUrl);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Unable to extract a valid stream URL from yt-dlp.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while streaming the track: {ex.Message}", "OK");
                }
            }
        }

        private async void OnPlayClicked(object sender, EventArgs e)
        {
            if (_selectedTrack == null)
            {
                await DisplayAlert("Error", "No track selected to play", "OK");
                return;
            }

            try
            {
                await CrossMediaManager.Current.Play();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred while playing the track: {ex.Message}", "OK");
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
                DisplayAlert("Error", $"An error occurred while pausing the track: {ex.Message}", "OK");
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
                DisplayAlert("Error", $"An error occurred while stopping the track: {ex.Message}", "OK");
            }
        }
    }
}