﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIDIparser.Models.VisualEventsSubclasses
{
    public enum CreateParamsEnum
    {
        spritePath = 0,
        posX,
        posY,
        posZ,
    }

    public class CreateDeleteVisualEvent : VisualEventBase
    {
        public CreateDeleteVisualEvent()
        {

        }
        public CreateDeleteVisualEvent (int objectID, long startTime, VisualEventTypeEnum type, string spritePath = null, float posX = 0, float posY = 0)
        {
            this.objectId = objectID;
            this.startTime = startTime;
            this.eventType = type;
            this.paramsList = new List<string> {spritePath,
            posX.ToString(), posY.ToString(), 0.0f.ToString()};
        }
        public override string ListRepresentation
        {
            get 
            {
                if(this.eventType == VisualEventTypeEnum.CreateObject)
                {
                    return objectId.ToString() + " " + eventType.ToString() + " " + startTime.ToString() + " " +
                    paramsList[0].Split('\\').Last() + " [" + paramsList[(int)CreateParamsEnum.posX] + "," + paramsList[(int)CreateParamsEnum.posY] +"]";
                }
                else
                    return objectId.ToString() + " " + eventType.ToString() + " " + startTime.ToString();
            }
        }
    }
}
