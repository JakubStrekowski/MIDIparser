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
using MIDIparser.EventMessages;

namespace MIDIparser.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly MidiEventsToTextParser _midiEventsTextParser;
        private DancerSong _dancerSong;


        private IEnumerable<MidiFile> midiChannels;
        private IEnumerable<Note> presentedNotes;
        private Note selectedNote;
        private ObservableCollection<Note> selectedNotes;
        private ObservableCollection<string> channelTitles;
        private string parsedNotes;
        private string selectedChannel;
        private int channelId;
        private float songPercentPlayed;

        private OutputDevice currentlyPlayingDevice;
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
        public ObservableCollection<string> ChannelTitles
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
        public Note SelectedNote
        {
            get { return selectedNote; }
            set
            {
                selectedNote = value;
                OnPropertyChange("SelectedNote");
            }
        }

        public ObservableCollection<Note> SelectedNotes
        {
            get
            {
                return selectedNotes;
            }
            set
            {

                (selectedNotes as Collection<Note>).Clear();
                foreach (Note model in value)
                {
                    (selectedNotes as Collection<Note>).Add(model);
                }
                OnPropertyChange("SelectedNotes");
            }
        }

        public string SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                SelectedNote = null;
                selectedChannel = value;
                OnPropertyChange("SelectedChannel");
                ChangeChannel();
            }
        }

        public string CurrentSongPlayTime
        {
            get
            {
                if(playback == null)
                {
                    return "00:00:00 - 00:00:00";
                }
                else
                {
                    return playback.GetCurrentTime(TimeSpanType.Metric).ToString() + " - " + playback.GetDuration(TimeSpanType.Metric).ToString();
                }
            }
        }

        public float SongPercentPlayed
        {
            get
            {
                if (playback == null)
                    return 0;
                else
                { 
                    return songPercentPlayed;
                }
            }
            set
            {
                if (playback == null)
                    return;
                else
                {
                    songPercentPlayed = value;
                    MusicalTimeSpan newTime = TimeConverter.ConvertTo<MusicalTimeSpan>((long)(((float)Convert.ToSingle(TimeConverter.ConvertFrom(playback.GetDuration(TimeSpanType.Midi), playback.TempoMap))) * songPercentPlayed/100f), playback.TempoMap);
                    playback.MoveToTime(newTime);
                    OnPropertyChange("SongPercentPlayed");
                }
            }
        }

        public MainWindowViewModel()
        {
            _midiEventsTextParser = new MidiEventsToTextParser();
        }

        #region commands
        private ICommand _cmdOpenFileClick;
        private ICommand _cmdPlayMidiClick;
        private ICommand _cmdStopMidiClick;
        private ICommand _cmdPauseMidiClick;

        public ICommand CmdOpenFileClick
        {
            get
            {
                if (_cmdOpenFileClick == null)
                {
                    _cmdOpenFileClick = new RelayCommand<ICommand>(x => OpenMidiFile());
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
                    _cmdPlayMidiClick = new RelayCommand<ICommand>(x => PlayMidiFile());
                }
                return _cmdPlayMidiClick;
            }
        }
        public ICommand CmdStopMidiClick
        {
            get
            {
                if (_cmdStopMidiClick == null)
                {
                    _cmdStopMidiClick = new RelayCommand<ICommand>(x => StopPlayMidiFile());
                }
                return _cmdStopMidiClick;
            }
        }
        public ICommand CmdPauseMidiClick
        {
            get
            {
                if (_cmdPauseMidiClick == null)
                {
                    _cmdPauseMidiClick = new RelayCommand<ICommand>(x => PausePlayMidiFile());
                }
                return _cmdPauseMidiClick;
            }
        }
        #endregion

        #region commandMethods
        private void OpenMidiFile()
        {
            _dancerSong = new DancerSong();
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MIDI files (*.mid)|*.mid;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedNote = null;
                _dancerSong.midi = MidiFile.Read(openFileDialog.FileName);
                ParsedNotes=_midiEventsTextParser.ParseFromMidFormat(_dancerSong.midi);
                channelId = 0;
                ChannelTitles = _midiEventsTextParser.ChannelTitles;
                PresentedNotes = _midiEventsTextParser.NotesInChannel[0];
                selectedNotes = new ObservableCollection<Note>();
                SplitByChannels();
                SelectedChannel = "All channels";
                EventSystem.Publish<OnMidiLoadedMessage>(
                    new OnMidiLoadedMessage { 
                        midiChannels = this.midiChannels,
                        playback = this.playback,
                        midiChannelsTitles = this.channelTitles
                    });
            }
        }

        private void ChangeChannel()
        {
            SelectedNote = null;
            if (string.Equals(SelectedChannel, "All channels"))
            {
                channelId = 0;
            }
            else
            {
                try
                {
                    channelId = Int32.Parse(SelectedChannel.Split(' ')[1]);
                }
                catch(NullReferenceException ex)
                {
                    channelId = 0;
                }
            }
            ParsedNotes = _midiEventsTextParser.GetNotesOfChannel(channelId);
            PresentedNotes = _midiEventsTextParser.NotesInChannel[channelId];

            currentlyPlayingDevice?.Dispose();
            if (playback != null)
            {
                PlaybackCurrentTimeWatcher.Instance.RemoveAllPlaybacks();
                PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged -= OnCurrentTimeChanged;
            }
             playback?.Dispose();

            currentlyPlayingDevice = OutputDevice.GetByName("Microsoft GS Wavetable Synth");
            if (channelId == 0)
            {
                playback = _dancerSong.midi.GetPlayback(currentlyPlayingDevice);
            }
            else
            {
                playback = midiChannels.ToArray()[channelId-1].GetPlayback(currentlyPlayingDevice);
            }
            PlaybackCurrentTimeWatcher.Instance.AddPlayback(playback, TimeSpanType.Midi);
            PlaybackCurrentTimeWatcher.Instance.CurrentTimeChanged += OnCurrentTimeChanged;
            OnPropertyChange("CurrentSongPlayTime");
            if(channelId == 0)
            {
                EventSystem.Publish<OnChannelChangeMessage>(
                    new OnChannelChangeMessage
                    {
                        channelID = this.channelId
                    });
            }
            else
            {
                EventSystem.Publish<OnChannelChangeMessage>(
                    new OnChannelChangeMessage
                    {
                        channelID = this.channelId - 1
                    });
            }
        }

        private void SplitByChannels()
        {
            midiChannels = _dancerSong.midi.SplitByChannel();
        }

        private void PlayMidiFile()
        {
            if (playback.IsRunning)
            {
                playback.Stop();
                PlaybackCurrentTimeWatcher.Instance.Stop();
            }
            else
            {
                Task.Run(StartPlayingMidi);
            }
        }

        private void StopPlayMidiFile()
        {
            if (playback != null && currentlyPlayingDevice!= null)
            {
                if(playback.IsRunning)
                playback.Stop();
                playback.MoveToStart();
                PlaybackCurrentTimeWatcher.Instance.Stop();
            }
        }
        private void PausePlayMidiFile()
        {
            if (playback != null && currentlyPlayingDevice != null)
            {
                if (playback.IsRunning)
                {
                    playback.Stop();
                    PlaybackCurrentTimeWatcher.Instance.Stop();
                }
            }
        }

        private async void StartPlayingMidi()
        {

            if (playback != null)
            {
                if (playback.IsRunning)
                {
                    return;
                }
            }
            if (midiChannels == null)
            {
                MessageBox.Show("MIDI file was not splitted by channels.");
                return;
            }
            try
            {
                if (!playback.IsRunning && currentlyPlayingDevice != null)
                {
                    playback.InterruptNotesOnStop = true;
                    playback.Start();
                    PlaybackCurrentTimeWatcher.Instance.Start();
                    while (playback.IsRunning)
                    {
                        Thread.Sleep(1000);
                        songPercentPlayed = (float)Convert.ToSingle(TimeConverter.ConvertFrom(playback.GetCurrentTime(TimeSpanType.Midi), playback.TempoMap)) / (float)Convert.ToSingle(TimeConverter.ConvertFrom(playback.GetDuration(TimeSpanType.Midi) , playback.TempoMap) / 100f);
                        OnPropertyChange("SongPercentPlayed");
                    }
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnCurrentTimeChanged(object sender, PlaybackCurrentTimeChangedEventArgs e)
        {
            foreach (var playbackTime in e.Times)
            {
                var time = (MidiTimeSpan)playbackTime.Time;

                foreach (Note n in presentedNotes)
                {
                    if(n.Time> time.TimeSpan && time.TimeSpan < n.Time + n.Length)
                    {
                        SelectedNote = n; break;
                    }
                }

                EventSystem.Publish<OnMusicIsPlayingMessage>(
                    new OnMusicIsPlayingMessage
                    {
                        currentPosition = time
                    });

                OnPropertyChange("CurrentSongPlayTime");
                Console.WriteLine($"Current time is {time}.");
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
