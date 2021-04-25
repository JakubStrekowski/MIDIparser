using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Models.VisualEventsSubclasses
{
    public class CreateDeleteVisualEvent : VisualEventBase
    {
        public CreateDeleteVisualEvent()
        {

        }
        public CreateDeleteVisualEvent (int objectID, long startTime, VisualEventTypeEnum type, string spritePath = null)
        {
            this.objectId = objectID;
            this.startTime = startTime;
            this.eventType = type;
            this.paramsList = new List<string> {spritePath};
        }
        public override string ListRepresentation
        {
            get 
            {
                if(this.eventType == VisualEventTypeEnum.CreateObject)
                {
                    return objectId.ToString() + " " + eventType.ToString() + " " + startTime.ToString() + " " +
                    paramsList[0].Split('\\').Last();
                }
                else
                    return objectId.ToString() + " " + eventType.ToString() + " " + startTime.ToString();
            }
        }
    }
}
