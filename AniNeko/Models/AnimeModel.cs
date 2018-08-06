using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace AniNeko.Models
{
    public class AnimeModel : PropertyChangedBase
    {
        private string _animeName;
        private int _currentEpisode;
        private int _totalEpisodes;
        private string _watchStatus;
        private int _rating;
        private float _percentComplete;
        private bool _hidden;

        public AnimeModel()
        {
            // Default values
            _currentEpisode = 0;
            _totalEpisodes = 0;
            _watchStatus = "Plan to Watch";
            _rating = 0;
        }

        public int Id { get; set; }

        public string AnimeName
        {
            get { return _animeName; }
            set
            {
                _animeName = value;
                NotifyOfPropertyChange(() => AnimeName);
            }
        }

        public int CurrentEpisode
        {
            get { return _currentEpisode; }
            set
            {
                _currentEpisode = value;
                NotifyOfPropertyChange(() => CurrentEpisode);
            }
        }

        public int TotalEpisodes
        {
            get { return _totalEpisodes; }
            set
            {
                _totalEpisodes = value;
                NotifyOfPropertyChange(() => TotalEpisodes);
            }
        }

        public string WatchStatus
        {
            get {
                return _watchStatus; }
            set
            {
                _watchStatus = value;
                NotifyOfPropertyChange(() => WatchStatus);
            }
        }

        public int Rating
        {
            get { return _rating; }
            set
            {
                _rating = value;
                NotifyOfPropertyChange(() => Rating);
            }
        }

        public List<String> Status
        {
            get { return new List<string> { "Watching", "Completed", "Plan to Watch", "Dropped" }; }
        }

        public float CalculatePercentComplete()
        {
            if (_totalEpisodes == 0)
            {
                _percentComplete = 0f;
            }
            else
            {
                _percentComplete = (float)_currentEpisode / _totalEpisodes;
            }

            return _percentComplete;
        }

        public float PercentComplete
        {
            get { return CalculatePercentComplete(); }
        }

        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden = value;
                NotifyOfPropertyChange(() => Hidden);
            }
        }

        public bool IsFlaggedForSearch { get; set; }
    }
}
