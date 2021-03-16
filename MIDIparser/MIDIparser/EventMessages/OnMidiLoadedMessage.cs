using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    class OnMidiLoadedMessage
    {
        public IEnumerable<MidiFile> midiChannels;
        public Playback playback;
        public ObservableCollection<string> midiChannelsTitles;
        public long ticksPerQuarterNote;
    }
}
