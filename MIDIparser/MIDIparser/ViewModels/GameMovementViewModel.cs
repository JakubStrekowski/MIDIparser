using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MIDIparser.EventMessages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.ViewModels
{
    class GameMovementViewModel : INotifyPropertyChanged
    {
        private IEnumerable<MidiFile> midiChannels;
        private long songLength;
        private long currentSongPosition;


        public long SongLength
        {
            get => songLength;
            set {
                songLength = value / 10;
                OnPropertyChange("SongLength");
            }
        }

        public long CurrentSongPosition {
            get => currentSongPosition;
            set {
                currentSongPosition = value / 10;
                OnPropertyChange("CurrentSongPosition");
           }
    }

    public GameMovementViewModel()
        {
            SongLength = 10240;
            CurrentSongPosition = 0;
            EventSystem.Subscribe<OnMidiLoadedMessage>(CalculateMaxLength);
            EventSystem.Subscribe<OnMusicIsPlayingMessage>(GetCurrentPosition);
        }

        #region EventHandlers
        public void CalculateMaxLength(OnMidiLoadedMessage msg)
        {
            midiChannels = msg.midiChannels;
            SongLength = TimeConverter.ConvertFrom(msg.playback.GetDuration(TimeSpanType.Midi), msg.playback.TempoMap);
        }
        public void GetCurrentPosition(OnMusicIsPlayingMessage msg)
        {
            CurrentSongPosition = msg.currentPosition;
        }
        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
