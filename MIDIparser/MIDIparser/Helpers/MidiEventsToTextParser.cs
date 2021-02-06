using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Helpers
{
    class MidiEventsToTextParser
    {
        private string parsedString;
        private readonly List<string> parsedStringByChannel;
        private ObservableCollection<string> channelTitles;

        public List<IEnumerable<Note>> NotesInChannel { get; protected set; }
        public string ParsedString
        {
            get
            {
                if (parsedString == "" || parsedString == null) throw new Exception("Parser didn't run parsing method yet.");
                else return parsedString;
            }
            protected set
            {
                parsedString = value;
            }
        }
        public ObservableCollection<string> ChannelTitles
        {
            get { return channelTitles; }
            private set { channelTitles = value; }
        }

        public MidiEventsToTextParser()
        {
            parsedString = "";
            parsedStringByChannel = new List<string>();
            ChannelTitles = new ObservableCollection<string>();
            NotesInChannel = new List<IEnumerable<Note>>();
        }

        public string ParseFromMidFormat(MidiFile midiFile)
        {
            parsedStringByChannel.Clear();
            ChannelTitles.Clear();
            NotesInChannel.Clear();
            int noteCount = 0;
            parsedString = "";
            IEnumerable<Note> notes = midiFile.GetNotes();
            {
                foreach (Note note in notes)
                {
                    noteCount++;
                    parsedString += noteCount + ". " + note.Time + " - " + note.Length + ": " + note.Octave + " " + note.NoteNumber + " " + note.NoteName + '\n';
                }
            }
            IEnumerable<FourBitNumber> channels = midiFile.GetChannels();

            ChannelTitles.Add("All channels");
            parsedStringByChannel.Add(parsedString);
            NotesInChannel.Add(notes);
            int channelNumber = 1;
            foreach(FourBitNumber channel in channels)
            {
                parsedStringByChannel.Add("");
                ChannelTitles.Add("Channel " + (channelNumber));
                IEnumerable<Note>notesInChannel = midiFile.GetNotes().Where(x => x.Channel == channel);
                NotesInChannel.Add(notesInChannel);
                noteCount = 0;
                foreach (Note note in notesInChannel)
                {
                    noteCount++;
                    parsedStringByChannel[channelNumber] += noteCount + ". " + note.Time + " - " + note.Length + ": " + note.Octave + " " + note.NoteNumber + " " + note.NoteName + '\n';
                }
                channelNumber++;
            }
            return parsedString;
        }

        public string GetNotesOfChannel(int channelId)
        {
            return parsedStringByChannel[channelId];
        }
    }
}
