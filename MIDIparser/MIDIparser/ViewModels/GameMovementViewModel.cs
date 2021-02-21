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
using MIDIparser.Views;

namespace MIDIparser.ViewModels
{
    class GameMovementViewModel : INotifyPropertyChanged
    {
        private IEnumerable<MidiFile> midiChannels;
        private long songLength;
        private long currentSongPosition;
        private int songPresentedSizeMultiplier;
        private int selectedChannel;


        public long SongLength
        {
            get => songLength;
            set {
                songLength = value / SongPresentedSizeMultiplier;
                OnPropertyChange("SongLength");
            }
        }

        public long CurrentSongPosition
        {
            get => currentSongPosition;
            set
            {
                currentSongPosition = value / SongPresentedSizeMultiplier;
                OnPropertyChange("CurrentSongPosition");
            }
        }

        public int SongPresentedSizeMultiplier
        {
            get => songPresentedSizeMultiplier;
            set
            {
                if (songPresentedSizeMultiplier != 0)
                {
                    songLength = (songLength * songPresentedSizeMultiplier);
                }
                songPresentedSizeMultiplier = value;
                SongLength = SongLength;
                if (midiChannels != null)
                {
                    RecalculateCanvasElements();
                }
                OnPropertyChange("SongPresentedSizeMultiplier");
            }
        }
        public int SelectedChannel
        {
            get => selectedChannel;
            set
            {
                selectedChannel = value;
                if (midiChannels != null)
                {
                    RecalculateCanvasElements();
                }
                OnPropertyChange("SelectedChannel");
            }
        }

        public GameMovementViewModel()
        {
            SongPresentedSizeMultiplier = 10;
            SongLength = 10240;
            CurrentSongPosition = 0;
            EventSystem.Subscribe<OnMidiLoadedMessage>(CalculateMaxLength);
            EventSystem.Subscribe<OnMusicIsPlayingMessage>(GetCurrentPosition);
            EventSystem.Subscribe<OnSongPreviewScaleChangeMessage>(GetNewScale);
            EventSystem.Subscribe<OnChannelChangeMessage>(GetSelectedChannel);
        }

        #region EventHandlers
        public void CalculateMaxLength(OnMidiLoadedMessage msg)
        {
            SongPresentedSizeMultiplier = 10;
            midiChannels = msg.midiChannels;
            SongLength = TimeConverter.ConvertFrom(msg.playback.GetDuration(TimeSpanType.Midi), msg.playback.TempoMap);
            RecalculateCanvasElements();
        }

        public void GetCurrentPosition(OnMusicIsPlayingMessage msg)
        {
            CurrentSongPosition = msg.currentPosition;
        }

        public void GetNewScale(OnSongPreviewScaleChangeMessage msg)
        {
            SongPresentedSizeMultiplier = msg.newScale;
        }
        public void GetSelectedChannel(OnChannelChangeMessage msg)
        {
            SelectedChannel = msg.channelID;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RecalculateCanvasElements()
        {
            int NOTE_AVG_AMMNT = 10;
            float currentBaseToneAvg;
            int movingAverageAmmount = 0;
            int toneSum = 0;
            Note[] notes = midiChannels.ToArray()[selectedChannel].GetNotes().ToArray();
            Note[] noteAvgGroup = new Note[NOTE_AVG_AMMNT];
            int noteGroupMin;
            int noteGroupMax;
            EventSystem.Publish<OnRedrawMusicMovesMessage>(
                new OnRedrawMusicMovesMessage
                { });
            for (int i = 0; i < notes.Count(); i++)
            {
                noteGroupMin = Int32.MaxValue;
                noteGroupMax = Int32.MinValue;
                Note note = notes[i];
                noteAvgGroup[i % NOTE_AVG_AMMNT] = note;
                if (movingAverageAmmount < NOTE_AVG_AMMNT)
                {
                    toneSum += note.NoteNumber * (note.Octave+1);
                    movingAverageAmmount++;
                    currentBaseToneAvg = (toneSum / movingAverageAmmount);
                    for(int j = 0; j <= i; j++)
                    {
                        if(noteAvgGroup[j].NoteNumber * (note.Octave + 1) < noteGroupMin)
                        {
                            noteGroupMin = noteAvgGroup[j].NoteNumber;
                        }
                        else if(noteAvgGroup[j].NoteNumber * (note.Octave + 1) > noteGroupMax)
                        {
                            noteGroupMax = noteAvgGroup[j].NoteNumber;
                        }
                    }
                }
                else
                {
                    toneSum -= notes[i - NOTE_AVG_AMMNT].NoteNumber * (notes[i - NOTE_AVG_AMMNT].Octave + 1);
                    toneSum += note.NoteNumber * (note.Octave + 1);
                    currentBaseToneAvg = (toneSum / NOTE_AVG_AMMNT);
                    for (int j = 0; j < NOTE_AVG_AMMNT; j++)
                    {
                        if (noteAvgGroup[j].NoteNumber * (note.Octave + 1) < noteGroupMin)
                        {
                            noteGroupMin = noteAvgGroup[j].NoteNumber * (note.Octave + 1);
                        }
                        else if (noteAvgGroup[j].NoteNumber * (note.Octave + 1) > noteGroupMax)
                        {
                            noteGroupMax = noteAvgGroup[j].NoteNumber * (note.Octave + 1);
                        }
                    }
                }
                if(note.NoteNumber * (note.Octave + 1) < currentBaseToneAvg)
                {
                    if(note.NoteNumber * (note.Octave + 1) == noteGroupMin)
                    {
                        EventSystem.Publish<OnCreateNoteElementMessage>(
                            new OnCreateNoteElementMessage
                            {
                                channelID = 3,
                                startTime = note.Time / SongPresentedSizeMultiplier,
                                duration = note.Length / SongPresentedSizeMultiplier 
                            });
                    }
                    else
                    {

                        EventSystem.Publish<OnCreateNoteElementMessage>(
                            new OnCreateNoteElementMessage
                            {
                                channelID = 2,
                                startTime = note.Time / SongPresentedSizeMultiplier,
                                duration = note.Length / SongPresentedSizeMultiplier 
                            });
                    }
                }
                else if(note.NoteNumber * (note.Octave + 1) >= currentBaseToneAvg)
                {
                    if (note.NoteNumber * (note.Octave + 1) == noteGroupMax)
                    {

                        EventSystem.Publish<OnCreateNoteElementMessage>(
                            new OnCreateNoteElementMessage
                            {
                                channelID = 0,
                                startTime = note.Time / SongPresentedSizeMultiplier,
                                duration = note.Length / SongPresentedSizeMultiplier 
                            });
                    }
                    else
                    {
                        EventSystem.Publish<OnCreateNoteElementMessage>(
                            new OnCreateNoteElementMessage
                            {
                                channelID = 1,
                                startTime = note.Time / SongPresentedSizeMultiplier,
                                duration = note.Length / SongPresentedSizeMultiplier
                            });
                    }
                }
            }
        }
    }
}
