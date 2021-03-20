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
using MIDIparser.Models;
using System.Xml.Serialization;
using System.IO;

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
                    songLength *= songPresentedSizeMultiplier;
                }
                songPresentedSizeMultiplier = value;
                SongLength = SongLength;
                if (midiChannels != null)
                {
                    //RecalculateCanvasElements();
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
            EventSystem.Subscribe<OnMovementChannelChangeMessage>(GetSelectedChannel);
            EventSystem.Subscribe<OnStartGeneratingMovesMessage>(StartGeneratingMoves);
            EventSystem.Subscribe<OnExportSendRawEventsMessage>(ExportMusicEvents);
        }

        #region EventHandlers
        public void CalculateMaxLength(OnMidiLoadedMessage msg)
        {
            SongPresentedSizeMultiplier = 10;
            midiChannels = msg.midiChannels;
            SongLength = TimeConverter.ConvertFrom(msg.playback.GetDuration(TimeSpanType.Midi), msg.playback.TempoMap);
            //RecalculateCanvasElements();
        }

        public void GetCurrentPosition(OnMusicIsPlayingMessage msg)
        {
            CurrentSongPosition = msg.currentPosition;
        }

        public void GetNewScale(OnSongPreviewScaleChangeMessage msg)
        {
            SongPresentedSizeMultiplier = msg.newScale;
        }
        public void GetSelectedChannel(OnMovementChannelChangeMessage msg)
        {
            SelectedChannel = msg.channelID;
        }
        public void StartGeneratingMoves(OnStartGeneratingMovesMessage msg)
        {
            RecalculateCanvasElements(msg.startTime, msg.endTime);
        }

        public void ExportMusicEvents(OnExportSendRawEventsMessage msg)
        {
            foreach(MusicMovementEvent musicEvent in msg.musicEvents.movementEvents)
            {
                musicEvent.RecalculateTime(SongPresentedSizeMultiplier, msg.maxMoveTapThreshold);
            }
            System.IO.TextWriter writer = new StreamWriter("test.xml");
            XmlSerializer xml = new XmlSerializer(typeof(DancerEvents));
            xml.Serialize(writer, msg.musicEvents);
            writer.Close();
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RecalculateCanvasElements(double startTime, double endTime)
        {
            int NOTE_AVG_AMMNT = 10;
            float currentBaseToneAvg;
            int movingAverageAmmount = 0;
            int toneSum = 0;
            int noteGroupMin;
            int noteGroupMax;
            startTime *= SongPresentedSizeMultiplier;
            endTime *= SongPresentedSizeMultiplier;

            List<Note> notes = midiChannels.ToArray()[selectedChannel].GetNotes().ToList();
            for(int i = 0; i< notes.Count; i++)
            {
                if(notes[i].Time < startTime || notes[i].Time > endTime)
                {
                    notes.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            if (NOTE_AVG_AMMNT > notes.Count) NOTE_AVG_AMMNT = notes.Count;

            if (NOTE_AVG_AMMNT == 0) return; //no notes found between set begin and end time

            noteGroupMin = Int32.MaxValue;
            noteGroupMax = Int32.MinValue;
            for (int i = 0; i < NOTE_AVG_AMMNT; i++)
            {
                Note note = notes[i];
                toneSum += note.NoteNumber * (note.Octave + 1);
                if (note.NoteNumber * (note.Octave + 1) < noteGroupMin)
                {
                    noteGroupMin = note.NoteNumber * (note.Octave + 1);
                }
                else if (note.NoteNumber * (note.Octave + 1) > noteGroupMax)
                {
                    noteGroupMax = note.NoteNumber * (note.Octave + 1);
                }
            }
            currentBaseToneAvg = (toneSum / NOTE_AVG_AMMNT);

            Note[] noteAvgGroup = new Note[NOTE_AVG_AMMNT];

            EventSystem.Publish<OnRedrawMusicMovesMessage>(
                new OnRedrawMusicMovesMessage
                {
                    posBegin = startTime / SongPresentedSizeMultiplier,
                    posEnd = endTime / SongPresentedSizeMultiplier
                });

            for (int i = 0; i < notes.Count(); i++)
            {
                Note note = notes[i];
                noteAvgGroup[i % NOTE_AVG_AMMNT] = note;
                if (movingAverageAmmount < NOTE_AVG_AMMNT)
                {
                    movingAverageAmmount++
                    ; // first group of notes is calculated before this loop
                }
                else
                {
                    noteGroupMin = Int32.MaxValue;
                    noteGroupMax = Int32.MinValue;
                    toneSum -= notes[i - NOTE_AVG_AMMNT].NoteNumber * (notes[i - NOTE_AVG_AMMNT].Octave + 1);
                    toneSum += note.NoteNumber * (note.Octave + 1);
                    currentBaseToneAvg = (toneSum / NOTE_AVG_AMMNT);
                    for (int j = 0; j < NOTE_AVG_AMMNT; j++)
                    {
                        if (noteAvgGroup[j].NoteNumber * (noteAvgGroup[j].Octave + 1) < noteGroupMin)
                        {
                            noteGroupMin = noteAvgGroup[j].NoteNumber * (noteAvgGroup[j].Octave + 1);
                        }
                        else if (noteAvgGroup[j].NoteNumber * (noteAvgGroup[j].Octave + 1) > noteGroupMax)
                        {
                            noteGroupMax = noteAvgGroup[j].NoteNumber * (noteAvgGroup[j].Octave + 1);
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
