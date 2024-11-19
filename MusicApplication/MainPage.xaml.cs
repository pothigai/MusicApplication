using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MusicApplication.Models;
using MusicApplication.Services;
using MediaManager;

namespace MusicApplication
{
    public partial class MainPage : ContentPage
    {
        private readonly UserSearch _userSearch;
        private readonly ObservableCollection<TrackInfo> _searchResults;
        private TrackInfo _selectedTrack;
        private readonly Player _player; 

        public MainPage()
        {
            InitializeComponent();
            _userSearch = new UserSearch();
            _searchResults = new ObservableCollection<TrackInfo>();
            _player = new Player();

            ResultsCollectionView.ItemsSource = _searchResults;
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            string keyword = SearchEntry.Text;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                ResultLabel.Text = "Searching...";
                _searchResults.Clear();

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
        }

        private async void OnTrackSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                _selectedTrack = (TrackInfo)e.CurrentSelection[0];
                MetadataLabel.Text = $"Title: {_selectedTrack.Title}\nArtist: {_selectedTrack.Artist}\nDuration: {_selectedTrack.Duration}";

                
                var ytDlpService = new YtDlpService();
                var streamUrl = await ytDlpService.GetAudioStreamUrlAsync(_selectedTrack.Url);

                if (!string.IsNullOrEmpty(streamUrl))
                {
                    
                    await _player.PlayAsync(_selectedTrack); 
                }
                else
                {
                    await DisplayAlert("Error", "Unable to stream this track", "OK");
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

           
            await _player.PlayAsync(_selectedTrack);
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            _player.Pause();
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            _player.Stop(); 
            MetadataLabel.Text = "";
        }
    }
}
