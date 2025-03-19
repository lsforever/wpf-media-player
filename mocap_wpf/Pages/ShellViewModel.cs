using MediaPlayer.Models;
using MediaPlayer.Services;
using mocap_wpf.Messages;
using Stylet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;

namespace mocap_wpf.Pages
{
    public class ShellViewModel : Screen,
        INotifyPropertyChanged, 
        IHandle<ResponseMediaPositionMessage>,
        IHandle<MediaDurationMessage>,
        IHandle<SeekPositionResponse>,
        IHandle<SeekStateMessage>
    {

        private readonly IEventAggregator _eventAggregator;
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.DisplayName = "Mocap Player";
            _mediaService = new MediaService();
            RecentFiles = _mediaService.GetRecentFiles();
            _eventAggregator.Subscribe(this);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;
        }

        // unsubscribe when ViewModel is closed
        protected override void OnDeactivate()
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate();
        }


        private readonly MediaService _mediaService;
        private MediaFile _currentMedia;
        private double _currentPosition = 0;
        private double _duration = 1;
        private bool _isPlaying;
        private readonly DispatcherTimer _timer;
        private bool _isSeeking = false;


        public MediaFile CurrentMedia
        {
            get => _currentMedia;
            set
            {
                _currentMedia = value;
                CurrentPosition = 0;
                IsPlaying = false;
                _eventAggregator.Publish(new PauseMediaMessage());
                NotifyOfPropertyChange(() => CurrentMedia);
                NotifyOfPropertyChange(() => CurrentPosition);
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public string PlayPauseBtnText
        {
            get => IsPlaying? "Pause" : "Play";
        }
        

        private MediaElement _mediaElement;

        public MediaElement MediaElement
        {
            get => _mediaElement;
            set
            {
                _mediaElement = value;
                NotifyOfPropertyChange(() => MediaElement);
            }
        }


        public double CurrentPosition
        {
            get => _currentPosition;
            set
            {
                if (_currentPosition != value)
                {
                    _currentPosition = value;
                    NotifyOfPropertyChange(() => CurrentPosition);
                    NotifyOfPropertyChange(() => CurrentPositionFormatted);
                }
            }
        }

        public string CurrentPositionFormatted => TimeSpan.FromSeconds(CurrentPosition).ToString(@"hh\:mm\:ss");

        public double Duration
        {
            get => _duration;
            set
            {
                _duration = value;
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                NotifyOfPropertyChange(() => IsPlaying);
                NotifyOfPropertyChange(() => PlayPauseBtnText);
                if (_isPlaying)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetCurrentPosition();
            NotifyOfPropertyChange(() => CurrentPosition);
        }

        public void UpdatePosition(double position)
        {
            if (!IsPlaying) return;
            _currentPosition = position;
            NotifyOfPropertyChange(() => CurrentPosition);
        }

        public void GetCurrentPosition()
        {
            _eventAggregator.Publish(new RequestMediaPositionMessage());
        }

        public void Handle(ResponseMediaPositionMessage message)
        {
            if (_isSeeking) return;
            CurrentPosition = message.Position.TotalSeconds;
            UpdatePosition(CurrentPosition);
        }

        public void Handle(MediaDurationMessage message)
        {
            Duration = message.Duration.TotalSeconds;
        }

        public void Handle(SeekPositionResponse message)
        {
            _eventAggregator.Publish(new SeekPositionMessage(TimeSpan.FromSeconds(CurrentPosition)));
        }
        
        public void Handle(SeekStateMessage message)
        {
            _isSeeking = message.IsSeeking;
        }

        public ObservableCollection<MediaFile> RecentFiles { get; }
        public void OpenFile()
        {
            var mediaFile = _mediaService.OpenMediaFile();
            if (mediaFile != null)
            {
                CurrentMedia = mediaFile;
                RecentFiles.Add(mediaFile);
                IsPlaying = false;
                CurrentPosition = 0;
            }
        }

        public void PlayPause()
        {
            if (CurrentMedia == null) { return; }

            IsPlaying = !IsPlaying;
            if (IsPlaying)
            {
                //PlayRequested?.Invoke(this, EventArgs.Empty);
                _eventAggregator.Publish(new PlayMediaMessage());
            }
            else
            {
                //PauseRequested?.Invoke(this, EventArgs.Empty);
                _eventAggregator.Publish(new PauseMediaMessage());
            }
        }

        public void Stop()
        {
            IsPlaying = false;
            CurrentPosition = 0;
            _eventAggregator.Publish(new StopMediaMessage());
        }

    }
}
