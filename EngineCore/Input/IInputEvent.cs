using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCore.Input
{
    public interface IInputEvent<T> where T : IInputEventData
    {
        abstract T Data { get; }
    }
}
