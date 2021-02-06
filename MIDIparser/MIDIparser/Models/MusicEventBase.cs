using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Models
{
    public enum EventTypeEnum
    {
        ArrowUpInstant,
        ArrowRightInstant,
        ArrowLeftInstant,
        ArrowDownInstant,
        ArrowUpDuration,
        ArrowRightDuration,
        ArrowLeftDuration,
        ArrowDownDuration,

        ChangeBackground,
    }

    [Serializable]
    public abstract class MusicEventBase
    {
        private EventTypeEnum eventTypeID;
        private long startTime;
        private long duration;

        protected EventTypeEnum EventTypeID { get => eventTypeID; set => eventTypeID = value; }
        protected long StartTime { get => startTime; set => startTime = value; }
        protected long Duration { get => duration; set => duration = value; }
    }
}
