using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Models
{
    [Serializable]
    public class DancerEvents
    {
        public Collection<MusicMovementEvent> movementEvents;
        public Collection<MusicEventBase> otherEvents; 

        public DancerEvents()
        {
            movementEvents = new Collection<MusicMovementEvent>();
            otherEvents = new Collection<MusicEventBase>();
        }
    }
}
