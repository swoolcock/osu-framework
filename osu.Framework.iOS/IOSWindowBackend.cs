// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Platform.Sdl;
using SDL2;

namespace osu.Framework.iOS
{
    public class IOSWindowBackend : Sdl2WindowBackend
    {
        protected override bool HandleCustomEvent(SDL.SDL_Event evt)
        {
            switch (evt.type)
            {
                case SDL.SDL_EventType.SDL_APP_DIDENTERBACKGROUND:
                    break;

                case SDL.SDL_EventType.SDL_APP_DIDENTERFOREGROUND:
                    break;

                case SDL.SDL_EventType.SDL_APP_WILLENTERBACKGROUND:
                    break;

                case SDL.SDL_EventType.SDL_APP_WILLENTERFOREGROUND:
                    break;

                case SDL.SDL_EventType.SDL_APP_LOWMEMORY:
                    break;

                case SDL.SDL_EventType.SDL_APP_TERMINATING:
                    break;
            }

            return false;
        }

        public override void Create()
        {
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, 8);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, 8);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, 8);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 0);

            base.Create();
        }

        public override void Run()
        {
            SDL.SDL_SysWMinfo info = new SDL.SDL_SysWMinfo();
            SDL.SDL_VERSION(out info.version);

            if (SDL.SDL_GetWindowWMInfo(SdlWindowHandle, ref info) == SDL.SDL_bool.SDL_TRUE)
                GLWrapper.DefaultFrameBuffer = (int)info.info.uikit.framebuffer;
            else
                Console.WriteLine("error!");

            base.Run();
        }
    }
}
