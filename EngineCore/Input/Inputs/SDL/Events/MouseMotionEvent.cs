using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace EngineCore.Input.Events
{
    public class MouseMotionEvent : IInputEvent<MouseMotionEventData>
    {
        public MouseMotionEvent(MouseMotionEventData data)
        {
            this.Data = data;
        }

        public MouseMotionEventData Data { get; }
    }

    public class MouseMotionEventData : IInputEventData
    {
        public MouseMotionEventData(Vector2 position)
        {
            this.Position = position;
        }

        public MouseMotionEventData(int x, int y)
        {
            this.Position = new Vector2(x, y);
        }

        public Vector2 Position { get; private set; } = Vector2.Zero;

    }
}
