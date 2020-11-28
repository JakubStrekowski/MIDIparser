using Melanchall.DryWetMidi.Core;
using Microsoft.Win32;
using MIDIparser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MIDIparser.Helpers;
using System.Collections.ObjectModel;

namespace MIDIparser.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private MidiEventsToTextParser _midiEventsTextParser;
        private DancerSong _dancerSong;

        private string parsedNotes;
        private string selectedChannel;
        private int channelId;
        public string ParsedNotes
        {
            get { return parsedNotes; }
            private set
            {
                parsedNotes = value;
                OnPropertyChange("ParsedNotes");
            }
        }
        public Collection<string> ChannelTitles
        {
            get { return _midiEventsTextParser.ChannelTitles ?? throw new ArgumentNullException("ChannelTitles"); }
        }

        public string SelectedChannel
        {
            get { return selectedChannel; }
            set { 
                selectedChannel = value;
                OnPropertyChange("SelectedChannel");
            }
        }

        public MainWindowViewModel()
        {
            _midiEventsTextParser = new MidiEventsToTextParser();
        }

        #region commands
        private ICommand _cmdOpenFileClick;
        private ICommand _cmdChangeChannelClick;

        public ICommand CmdOpenFileClick
        {
            get
            {
                if (_cmdOpenFileClick == null)
                {
                    _cmdOpenFileClick = new RelayCommand<ICommand>(x => OpenMidiFile(x));
                }
                return _cmdOpenFileClick;
            }
        }
        public ICommand CmdChangeChannelClick
        {
            get
            {
                if (_cmdChangeChannelClick == null)
                {
                    _cmdChangeChannelClick = new RelayCommand<ICommand>(x => ChangeChannel(x));
                }
                return _cmdChangeChannelClick;
            }
        }
        #endregion

        #region commandMethods
        private void OpenMidiFile(object item)
        {
            _dancerSong = new DancerSong();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MIDI files (*.mid)|*.mid;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _dancerSong.midi = MidiFile.Read(openFileDialog.FileName);
                ParsedNotes=_midiEventsTextParser.ParseFromMidFormat(_dancerSong.midi);
            }
        }
        private void ChangeChannel(object item)
        {
            channelId = Int32.Parse(SelectedChannel.Split(' ')[1]);
            ParsedNotes = _midiEventsTextParser.GetNotesOfChannel(channelId-1);
        }
        #endregion



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
