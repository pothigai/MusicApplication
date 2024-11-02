using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using MusicApplication.Models;
using System.Threading.Tasks;

namespace MusicApplication
{
    public partial class MainPage : ContentPage
    {
        private readonly UserSearch _userSearch;
        private readonly ObservableCollection<TrackInfo> _searchResults;
        private readonly Player _player;

        public MainPage()
        {
            InitializeComponent();
            _userSearch = new UserSearch();
            _player = new Player();
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

                var resultsUrl = await _userSearch.DisplayResults(keyword);

                if (resultsUrl != null)
                {
                    _searchResults.Add(new TrackInfo
                    {
                        Title = "Sample Track",
                        Artist = "Sample Artist",
                        Duration = TimeSpan.FromMinutes(3),
                        Url = resultsUrl,
                        UniqueId = Guid.NewGuid().ToString()
                    });

                    ResultLabel.Text = "Select a track from the list";
                }
                else
                {
                    ResultLabel.Text = "No results found";
                }
            }
        }

        private void OnTrackSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedTrack = (TrackInfo)e.CurrentSelection[0];
                MetadataLabel.Text = $"Title: {selectedTrack.Title}\nArtist: {selectedTrack.Artist}\nDuration: {selectedTrack.Duration}";

                _player.Play(selectedTrack);
            }
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            if (_player.CurrentStatus == null)
            {
                DisplayAlert("Error", "No track selected to play", "OK");
                return;
            }

            if (_player.CurrentStatus.AudioStatus == PlayerStatus.Status.paused)
            {
                _player.Resume();
            }
            else
            {
                _player.Play(_player.CurrentStatus.Track);
            }
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
