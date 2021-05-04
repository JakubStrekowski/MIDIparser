using Microsoft.Win32;
using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MIDIparser.ViewModels
{
    class ExportSettingsViewModel : INotifyPropertyChanged
    {
        private long maxMoveTapThreshold;
        private string pathToMusic;
        private string pathToImage;
        private string songTitle;
        private string description;
        private ImageSource imageSourcePreview;

        public ImageSource ImageSourcePreview
        {
            get => imageSourcePreview;
            set
            {
                imageSourcePreview = value;
                OnPropertyChange("ImageSourcePreview");
            }
        }

        public long MaxMoveTapThreshold { get => maxMoveTapThreshold; 
            set
            {
                maxMoveTapThreshold = value;
                OnPropertyChange("MaxMoveTapThreshold");
            }
        }

        public string PathToMusic
        {
            get => pathToMusic;
            set
            {
                pathToMusic = value;
                OnPropertyChange("PathToMusic");
            }
        }
        public string PathToImage
        {
            get => pathToImage;
            set
            {
                pathToImage = value;
                OnPropertyChange("PathToImage");
            }
        }
        public string SongTitle
        {
            get => songTitle;
            set
            {
                songTitle = value;
                OnPropertyChange("SongTitle");
            }
        }
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChange("Description");
            }
        }


        public ExportSettingsViewModel()
        {
            MaxMoveTapThreshold = 720;
        }

        #region commands
        private ICommand _cmdExportClick;
        private ICommand _cmdSelectMusicClick;
        private ICommand _cmdSelectpreviewClick;

        public ICommand CmdExportClick
        {
            get
            {
                if (_cmdExportClick == null)
                {
                    _cmdExportClick = new RelayCommand<ICommand>(x => StartExporting());
                }
                return _cmdExportClick;
            }
        }
        public ICommand CmdSelectMusicClick
        {
            get
            {
                if (_cmdSelectMusicClick == null)
                {
                    _cmdSelectMusicClick = new RelayCommand<ICommand>(x => SelectMusicFile());
                }
                return _cmdSelectMusicClick;
            }
        }
        public ICommand CmdSelectPreviewClick
        {
            get
            {
                if (_cmdSelectpreviewClick == null)
                {
                    _cmdSelectpreviewClick = new RelayCommand<ICommand>(x => SelectImageFile());
                }
                return _cmdSelectpreviewClick;
            }
        }

        #endregion

        #region commandMethods
        private void StartExporting()
        {

            EventSystem.Publish(
                new OnStartExportMessage
                {
                    MaxLengthToTap = MaxMoveTapThreshold,
                    musicFilePath = PathToMusic,
                    imageFilePath = pathToImage,
                    title = songTitle,
                    description = this.description
                });
        }
        private void SelectMusicFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MIDI files (*.mid)|*.mid;|Music files(*.mp3;*.wav;*.ogg)|*.mp3;*.wav;*.ogg",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                PathToMusic = openFileDialog.FileName;
            }
        }
        private void SelectImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                PathToImage = openFileDialog.FileName;
                ImageSourcePreview = new BitmapImage(new Uri(PathToImage));
            }
        }


        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
