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
            , string imagePath = null, float posX = 0, float posY = 0)
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
                    return new CreateDeleteVisualEvent(objectID, startTime, type, imagePath, posX , posY);
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
            if (type == VisualEventTypeEnum.ChangeColorObjectLinear && allEvents.Exists(x => x.objectId == objectID) && !(allEvents.Exists(x => x.objectId == objectID && x.eventType == VisualEventTypeEnum.DeleteObject)))
            {
                return new ChangeColorLinearVisualEffect(objectID, startTime, type, colorToSet, timeToSet);
            }
            else
            {
                throw new Exception("Incorrect type, object doesn't exist or object already deleted");
            }
        }

        public static VisualEventBase InstantiateChangeColorArcEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents,
            ArgbColor colorToSet, long timeToSet)
        {
            if (type == VisualEventTypeEnum.ChangeColorObjectArc && allEvents.Exists(x => x.objectId == objectID) && !(allEvents.Exists(x => x.objectId == objectID && x.eventType == VisualEventTypeEnum.DeleteObject)))
            {
                return new ChangeColorArcVisualEffect(objectID, startTime, type, colorToSet, timeToSet);
            }
            else
            {
                throw new Exception("Incorrect type, object doesn't exist or object already deleted");
            }
        }

        public static VisualEventBase InstantiateChangePositionLinearEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents,
            long timeToSet, float posX, float posY)
        {
            if (type == VisualEventTypeEnum.ChangePosObjectLinear && allEvents.Exists(x => x.objectId == objectID) && !(allEvents.Exists(x=> x.objectId == objectID && x.eventType == VisualEventTypeEnum.DeleteObject)))
            {
                return new ChangePositionLinearVisualEffect(objectID, startTime, type, timeToSet, posX, posY);
            }
            else
            {
                throw new Exception("Incorrect type, object doesn't exist or object already deleted");
            }
        }

        public static VisualEventBase InstantiateChangePositionDampingEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents,
            long timeToSet, float posX, float posY)
        {
            if (type == VisualEventTypeEnum.ChangePosObjectArc && allEvents.Exists(x => x.objectId == objectID) && !(allEvents.Exists(x => x.objectId == objectID && x.eventType == VisualEventTypeEnum.DeleteObject)))
            {
                return new ChangePositionDampingVisualEffect(objectID, startTime, type, timeToSet, posX, posY);
            }
            else
            {
                throw new Exception("Incorrect type, object doesn't exist or object already deleted");
            }
        }

        public static VisualEventBase InstantiateChangeRotationLinearEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents,
            long timeToSet, float rotation)
        {
            if (type == VisualEventTypeEnum.ChangeRotObjectLinear && allEvents.Exists(x => x.objectId == objectID) && !(allEvents.Exists(x => x.objectId == objectID && x.eventType == VisualEventTypeEnum.DeleteObject)))
            {
                return new ChangeRotationLinearVisualEffect(objectID, startTime, type, timeToSet, rotation);
            }
            else
            {
                throw new Exception("Incorrect type, object doesn't exist or object already deleted");
            }
        }

        public static VisualEventBase InstantiateChangeRotationArcEvent(int objectID, long startTime, VisualEventTypeEnum type, List<VisualEventBase> allEvents,
            long timeToSet, float rotation)
        {
            if (type == VisualEventTypeEnum.ChangeRotObjectArc && allEvents.Exists(x => x.objectId == objectID) && !(allEvents.Exists(x => x.objectId == objectID && x.eventType == VisualEventTypeEnum.DeleteObject)))
            {
                return new ChangeRotationArcVisualEffect(objectID, startTime, type, timeToSet, rotation);
            }
            else
            {
                throw new Exception("Incorrect type, object doesn't exist or object already deleted");
            }
        }
    }

}
