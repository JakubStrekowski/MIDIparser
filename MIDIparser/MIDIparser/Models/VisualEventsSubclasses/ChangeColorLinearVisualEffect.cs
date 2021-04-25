using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Models.VisualEventsSubclasses
{
    public enum VisualEventsSubclassesParamsEnum
    {
        red,
        green,
        blue,
        alpha,
        timeToReach
    }

    public class ChangeColorLinearVisualEffect : VisualEventBase
    {
        public ChangeColorLinearVisualEffect()
        {

        }
        public ChangeColorLinearVisualEffect(int objectID, long startTime, VisualEventTypeEnum type, ArgbColor colorToSet, long timeToSet)
        {
            this.objectId = objectID;
            this.startTime = startTime;
            this.eventType = type;
            this.paramsList = new List<string>
            {
                colorToSet.red.ToString(),
                colorToSet.green.ToString(),
                colorToSet.blue.ToString(),
                colorToSet.alpha.ToString(),
                timeToSet.ToString()
            };
        }
        public override string ListRepresentation
        {
            get { return objectId.ToString() + " " + eventType.ToString() + " " + startTime.ToString() + " a" + paramsList[3] + " r" + paramsList[0] + " g" + paramsList[1] 
                    + " b" + paramsList[2]; }
        }
    }
}
