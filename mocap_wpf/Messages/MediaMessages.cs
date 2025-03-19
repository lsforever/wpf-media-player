using System;

namespace mocap_wpf.Messages
{
    public class PlayMediaMessage { }
    public class PauseMediaMessage { }
    public class StopMediaMessage { }

    public class RequestMediaPositionMessage { }

    public class ResponseMediaPositionMessage
    {
        public TimeSpan Position { get; }

        public ResponseMediaPositionMessage(TimeSpan position)
        {
            Position = position;
        }
    }

    public class MediaDurationMessage
    {
        public TimeSpan Duration { get; }

        public MediaDurationMessage(TimeSpan duration)
        {
            Duration = duration;
        }
    }

    public class SeekPositionMessage
    {
        public TimeSpan NewPosition { get; }

        public SeekPositionMessage(TimeSpan newPosition)
        {
            NewPosition = newPosition;
        }
    }

    public class SeekStateMessage
    {
        public Boolean IsSeeking { get; }

        public SeekStateMessage(Boolean isSeeking)
        {
            IsSeeking = isSeeking;
        }
    }

    public class SeekPositionResponse
    {
        public TimeSpan NewPosition { get; }

        public SeekPositionResponse(TimeSpan newPosition)
        {
            NewPosition = newPosition;
        }
    }
    
}
