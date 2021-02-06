using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Models
{
    class DancerSong
    {
        public MidiFile midi; //unparsed midi events
        public string musicFilePath; //path to a music file
        public DancerEvents dancerEvents;

        public DancerSong()
        {
            dancerEvents = new DancerEvents();
        }
    }
}
