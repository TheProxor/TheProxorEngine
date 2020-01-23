using System;
using System.Collections.Generic;
using System.Text;

namespace EngineCore.Input
{
    public static class InputHandler
    {
        public static List<Input> Inputs { get; private set; } = new List<Input>();

        public static void HandleLoop()
        {
            for (int i = 0; i < Inputs.Count; i++)
                Inputs[i].HandleLoop();
        }

        public static void AddInput(Input input)
        {
            Inputs.Add(input);
        }

        public static void RemoveInput(Input input)
        {
            Inputs.Remove(input);
        }
    }
}
