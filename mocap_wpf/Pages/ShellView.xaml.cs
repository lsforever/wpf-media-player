using mocap_wpf.Messages;
using Stylet;
using System;
using System.Windows;
using System.Windows.Input;

namespace mocap_wpf.Pages
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, 
        IHandle<PlayMediaMessage>, 
        IHandle<PauseMediaMessage>, 
        IHandle<StopMediaMessage>, 
        IHandle<RequestMediaPositionMessage>,
        IHandle<SeekPositionMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _isSeeking = false;
        private bool IsSeeking
        {
            get => _isSeeking;
            set
            {
                _isSeeking = value;
                _eventAggregator.Publish(new SeekStateMessage(_isSeeking));
            }
        }


        public ShellView(IEventAggregator eventAggregator)
        {
            this.InitializeComponent();
            _eventAggregator = eventAggregator;

            _eventAggregator.Subscribe((IHandle)this);

        }

        public void Handle(PlayMediaMessage message)
        {
            MediaPlayer.Play();
        }

        public void Handle(PauseMediaMessage message)
        {
            MediaPlayer.Pause();
        }

        public void Handle(StopMediaMessage message)
        {
            MediaPlayer.Stop();
        }

        protected override void OnClosed(EventArgs e)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnClosed(e);
        }

        public void Handle(RequestMediaPositionMessage message)
        {
            _eventAggregator.Publish(new ResponseMediaPositionMessage(MediaPlayer.Position));
        }

        public void Handle(SeekPositionMessage message)
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Position = message.NewPosition;
            }
        }


        // This event fires while the user is dragging the slider
        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsSeeking)
            {
                // Prevent seeking until the mouse is released
                return;
            }

        }

        // This event fires when the user releases the mouse
        private void SeekSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsSeeking = false;
            var seekPosition = TimeSpan.FromSeconds(PositionSlider.Value);
            _eventAggregator.Publish(new SeekPositionResponse(seekPosition));
        }

        // When the user starts moving the slider
        private void SeekSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSeeking = true;
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = MediaPlayer.NaturalDuration.TimeSpan;
                _eventAggregator.Publish(new MediaDurationMessage(duration));
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
