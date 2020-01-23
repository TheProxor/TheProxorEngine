using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCore.Input.Events
{
    public class CloseEvent : IInputEvent<CloseEventData>
    {
        public CloseEvent(CloseEventData data)
        {
            this.Data = data;
        }

        public CloseEventData Data { get; }
    }

    public class CloseEventData : IInputEventData
    {
        public CloseEventData(string log)
        {
            this.log = log;
        }

        public string log { get; private set; } = string.Empty;
    }
}
