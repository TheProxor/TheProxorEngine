using System;
using System.Collections.Generic;
using System.Text;

using SDL2;
using EngineCore.Input.Events;

namespace EngineCore.Input
{
    internal class InputSDL : Input
    {
        public InputSDL()
        {
            sdlEvent = new SDL.SDL_Event();
        }

        private SDL.SDL_Event sdlEvent;

        public Action<CloseEvent> OnClose { get; set; }

        public override void HandleLoop()
        {
            SDL.SDL_PumpEvents();

            while (SDL.SDL_PollEvent(out sdlEvent) != 0)
            {
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        OnClose?.Invoke(new CloseEvent(new CloseEventData("SDL_QUIT")));
                        break;
                   /* case SDL.SDL_EventType.SDL_MOUSEMOTION:
                        Dev.OnMouseMove(sdlEvent.motion.x, sdlEvent.motion.y);
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        switch (sdlEvent.button.button)
                        {
                            case 1:
                                Dev.OnMouseButton(NuklearEvent.MouseButton.Left, sdlEvent.button.x, sdlEvent.button.y, true);
                                break;
                            case 2:
                                Dev.OnMouseButton(NuklearEvent.MouseButton.Middle, sdlEvent.button.x, sdlEvent.button.y, true);
                                break;
                            case 3:
                                Dev.OnMouseButton(NuklearEvent.MouseButton.Right, sdlEvent.button.x, sdlEvent.button.y, true);
                                break;
                        }
                        Dev.OnMouseButton(NuklearEvent.MouseButton.Left, sdlEvent.motion.x, sdlEvent.motion.y, true);
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                        switch (sdlEvent.button.button)
                        {
                            case 1:
                                Dev.OnMouseButton(NuklearEvent.MouseButton.Left, sdlEvent.button.x, sdlEvent.button.y, false);
                                break;
                            case 2:
                                Dev.OnMouseButton(NuklearEvent.MouseButton.Middle, sdlEvent.button.x, sdlEvent.button.y, false);
                                break;
                            case 3:
                                Dev.OnMouseButton(NuklearEvent.MouseButton.Right, sdlEvent.button.x, sdlEvent.button.y, false);
                                break;
                        }
                        break;*/
                }
            }
        }
    }
}
