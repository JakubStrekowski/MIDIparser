using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    class OnCreateNoteElementMessage
    {
        public int channelID;
        public long startTime;
        public long duration;
    }
}
