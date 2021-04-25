using MIDIparser.Models;
using MIDIparser.Models.VisualEventsSubclasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Helpers
{
    class VisualEffectsFactory
    {
        public static VisualEventBase InstantiateCreateDestroyEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents
            , string imagePath = null)
        {
            if(type == VisualEventTypeEnum.CreateObject)
            {
                if(allEvents.Exists(x=>x.objectId == objectID))
                {
                    throw new Exception("Object with this ID already exists");
                }
                else
                {
                    if(imagePath == "" || imagePath is null) throw new Exception("Sprite object has to have any image attached");
                    return new CreateDeleteVisualEvent(objectID, startTime, type, imagePath);
                }
            }
            if (type == VisualEventTypeEnum.DeleteObject)
            {
                if (allEvents.Exists(x => x.objectId == objectID && (x.startTime > startTime || x.eventType == VisualEventTypeEnum.DeleteObject) ))
                {
                    throw new Exception("Destroy event has to be the last event referring to this object");
                }
                else
                {
                    return new CreateDeleteVisualEvent(objectID, startTime, type);
                }
            }
            else throw new Exception("Incorrect event type for this method");
        }

        public static VisualEventBase InstantiateChangeColorLinearEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents,
            ArgbColor colorToSet, long timeToSet)
        {
            if (type == VisualEventTypeEnum.ChangeColorObjectLinear && allEvents.Exists(x => x.objectId == objectID))
            {
                return new ChangeColorLinearVisualEffect(objectID, startTime, type, colorToSet, timeToSet);
            }
            else
            {
                throw new Exception("Incorrect type or object doesn't exist");
            }
        }
    }

}
