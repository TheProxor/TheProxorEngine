#define WINDOWS
#define LINUX
#define OSX

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Numerics;

using SDL2;
using NuklearDotNet;

using EngineEditor.GUI.GUI_Contexts;
using EngineCore.Input;

namespace EngineEditor.GUI
{
    public enum Mode : uint
    {
        WINDOWED                = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE,
        FULLSCREEN              = SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN,
        FULLSCREEN_BORDERLESS   = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS
    }

    public static class Conversions
    {
      
    }

    public sealed class Window
    {
        public IntPtr Handle                        { get; private set; }

        public int Width                            { get; private set; }
        public int Height                           { get; private set; }

        public int DisplayWidth;
        public int DisplayHeight;                   


        public Mode Mode                            { get; private set; }

        internal GL_GUI_Context WindowContext       { get; private set; }

        public string Title                         { get; private set; }

        public uint ID                              { get => SDL.SDL_GetWindowID(Handle); }


        public enum WindowState : byte
        {
            Open    = 0,
            Close   = 1
        }

        public WindowState CurrentWindowState       { get; private set; }

        public Window(string title, int width, int height, Mode mode)
        {
            this.Width = width;
            this.Height = height;
            this.Mode = mode;
            this.Title = title;         
        }

        public Vector2 AspectRatio                    { get; private set; }


        public void Open()
        {
            Handle = SDL.SDL_CreateWindow(Title, 40, 40, Width, Height, (SDL.SDL_WindowFlags)Mode | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            SDL.SDL_SetWindowResizable(Handle, SDL.SDL_bool.SDL_FALSE);
            SDL.SDL_GL_GetDrawableSize(Handle, out DisplayWidth, out DisplayHeight);
            AspectRatio = new Vector2((float)Width / Height, (float)Height / Width);

            SDL.SDL_Event sdlEvent;
            CurrentWindowState = WindowState.Open;

            var Dev = new GL_GUI_Context(this, Width, Height);

            Stopwatch SWatch = Stopwatch.StartNew();
            /* RWind.Closed += (S, E) => RWind.Close();
             RWind.MouseButtonPressed += (S, E) => Dev.OnMouseButton((NuklearEvent.MouseButton)E.Button, E.X, E.Y, true);
             RWind.MouseButtonReleased += (S, E) => Dev.OnMouseButton((NuklearEvent.MouseButton)E.Button, E.X, E.Y, false);
             RWind.MouseMoved += (S, E) => ;
             RWind.MouseWheelMoved += (S, E) => Dev.OnScroll(0, E.Delta);
             RWind.KeyPressed += (S, E) => OnKey(Dev, E, true);
             RWind.KeyReleased += (S, E) => OnKey(Dev, E, false);
             RWind.TextEntered += (S, E) => Dev.OnText(E.Unicode);*/


            Shared.Init(Dev);
            float Dt = 0.1f;
            while (CurrentWindowState != WindowState.Close)
            {
                SDL.SDL_PumpEvents();
           
                while (SDL.SDL_PollEvent(out sdlEvent) != 0)
                {
                    switch (sdlEvent.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            CurrentWindowState = WindowState.Close;
                            break;
                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
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
                            break;
                    }
                }

                Shared.DrawLoop(Dt);
                Dt = SWatch.ElapsedMilliseconds / 1000.0f;
                SWatch.Restart();
            }
        }

        public void Hide()
        {
            SDL.SDL_HideWindow(Handle);
        }

        ~Window()
        {
            GC.Collect();
        }
    }
}
