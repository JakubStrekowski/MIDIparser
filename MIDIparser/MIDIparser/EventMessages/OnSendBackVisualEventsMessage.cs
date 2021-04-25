using MIDIparser.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    class OnSendBackVisualEventsMessage
    {
        public Collection<VisualEventBase> allVisualEvents;
    }
}
