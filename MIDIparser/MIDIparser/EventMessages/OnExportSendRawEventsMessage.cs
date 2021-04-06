using MIDIparser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    class OnExportSendRawEventsMessage
    {
        public DancerEvents musicEvents;
        public long maxMoveTapThreshold;
        public string musicFilePath;
        public string imageFilePath;
        public string title;
        public string description;
    }
}
