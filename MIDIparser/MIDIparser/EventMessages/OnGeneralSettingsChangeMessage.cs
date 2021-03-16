using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    class OnGeneralSettingsChangeMessage
    {
        public bool scrollToCursor;
        public bool snapToGrid;
        public long gridSize;
        public long gridPadding;
    }
}
