using MediaPlayer.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;

namespace MediaPlayer.Services
{
    public class MediaService
    {
        public MediaFile OpenMediaFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Media files (*.mp3;*.mp4;*.wav)|*.mp3;*.mp4;*.wav|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return new MediaFile(openFileDialog.FileName);
            }

            return null;
        }

        public ObservableCollection<MediaFile> GetRecentFiles()
        {
            // Here you could implement loading recently played files from settings or local storage
            return new ObservableCollection<MediaFile>();
        }
    }
}