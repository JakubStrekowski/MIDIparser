﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MIDIparser.Models
{
    [Serializable]
    [XmlInclude(typeof(MusicEventBase))]
    [XmlInclude(typeof(MusicMovementEvent))]
    public class DancerEvents
    {
        [XmlArray("MovementEvents")]
        [XmlArrayItem("MusicMovementEvent")]
        public Collection<MusicMovementEvent> movementEvents;
        public Collection<MusicEventBase> otherEvents;
        [XmlArrayItem("MusicVisualEvent")]
        public Collection<VisualEventBase> visualEvents;

        public DancerEvents()
        {
            movementEvents = new Collection<MusicMovementEvent>();
            otherEvents = new Collection<MusicEventBase>();
            visualEvents = new Collection<VisualEventBase>();
        }
    }
}
