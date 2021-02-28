using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.EventMessages
{
    public enum PreviewControlEnum
    {
        None,
        SetBeginTime,
        SetEndTime,
        CreateNewMove,
        EditExistingMove,
        DeleteMove
    }
    class OnChangePreviewControlMessage
    {
        public PreviewControlEnum currentControl;
    }
}
