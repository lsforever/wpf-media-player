using System;

namespace MediaPlayer.Models
{
    public class MediaFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public TimeSpan Duration { get; set; }
        public string FileType { get; set; }

        public MediaFile(string filePath)
        {
            FilePath = filePath;
            FileName = System.IO.Path.GetFileName(filePath);
            FileType = System.IO.Path.GetExtension(filePath);
        }
    }
}