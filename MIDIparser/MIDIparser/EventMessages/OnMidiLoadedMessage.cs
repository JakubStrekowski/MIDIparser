using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    class OnMidiLoadedMessage
    {
        public IEnumerable<MidiFile> midiChannels;
        public Playback playback;
    }
}
