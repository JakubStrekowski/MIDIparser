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
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Tools;
using System.Threading;
using Melanchall.DryWetMidi.Interaction;

namespace MIDIparser.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private MidiEventsToTextParser _midiEventsTextParser;
        private DancerSong _dancerSong;


        private OutputDevice currentlyPlayingDevice;
        private IEnumerable<MidiFile> midiChannels;
        private IEnumerable<Note> presentedNotes;
        private Collection<string> channelTitles;
        private string parsedNotes;
        private string selectedChannel;
        private int channelId;

        private Playback playback;

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
            get { return channelTitles; }
            set
            {
                channelTitles = value;
                OnPropertyChange("ChannelTitles");
            }
        }
        public IEnumerable<Note> PresentedNotes
        {
            get { return presentedNotes;}
            set
            {
                presentedNotes = value;
                OnPropertyChange("PresentedNotes");
            }
        }

        public string SelectedChannel
        {
            get { return selectedChannel; }
            set { 
                selectedChannel = value;
                OnPropertyChange("SelectedChannel");
                ChangeChannel();
            }
        }

        public MainWindowViewModel()
        {
            _midiEventsTextParser = new MidiEventsToTextParser();
        }

        #region commands
        private ICommand _cmdOpenFileClick;
        private ICommand _cmdPlayMidiClick;
        private ICommand _cmdStopidiClick;

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
        public ICommand CmdPlayMidiClick
        {
            get
            {
                if (_cmdPlayMidiClick == null)
                {
                    _cmdPlayMidiClick = new RelayCommand<ICommand>(x => PlayMidiFile(x));
                }
                return _cmdPlayMidiClick;
            }
        }
        public ICommand CmdStopMidiClick
        {
            get
            {
                if (_cmdStopidiClick == null)
                {
                    _cmdStopidiClick = new RelayCommand<ICommand>(x => StopPlayMidiFile());
                }
                return _cmdStopidiClick;
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
                channelId = 0;
                ChannelTitles = _midiEventsTextParser.ChannelTitles;
                PresentedNotes = _midiEventsTextParser.NotesInChannel[0];
                SplitByChannels();
            }
        }

        private void ChangeChannel()
        {
            if(string.Equals(SelectedChannel, "All channels"))
            {
                channelId = 0;
            }
            else
            {
                channelId = Int32.Parse(SelectedChannel.Split(' ')[1]);
            }
            ParsedNotes = _midiEventsTextParser.GetNotesOfChannel(channelId);
            PresentedNotes = _midiEventsTextParser.NotesInChannel[channelId];
        }

        private void SplitByChannels()
        {
            midiChannels = _dancerSong.midi.SplitByChannel();
        }

        private void PlayMidiFile(object item)
        {
            Task.Run(StartPlayingMidi);
        }

        private void StopPlayMidiFile()
        {
            if (playback != null && currentlyPlayingDevice!= null)
            {
                if(playback.IsRunning)
                playback.Stop();
            }
        }

        private async void StartPlayingMidi()
        {
            if (midiChannels == null)
            {
                MessageBox.Show("MIDI file was not splitted by channels.");
                return;
            }
            try
            {
                currentlyPlayingDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
                if (channelId == 0)
                {
                    playback = _dancerSong.midi.GetPlayback(currentlyPlayingDevice);
                }
                else
                {
                    playback = midiChannels.ToArray()[channelId - 1].GetPlayback(currentlyPlayingDevice);
                }
                if (!playback.IsRunning && currentlyPlayingDevice != null)
                {
                    playback.InterruptNotesOnStop = true;
                    playback.Start();
                    while (playback.IsRunning)
                    {
                        Thread.Sleep(1000);
                    }
                }
                currentlyPlayingDevice.Dispose();
                playback.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
