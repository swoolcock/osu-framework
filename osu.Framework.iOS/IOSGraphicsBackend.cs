// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.OpenGL;
using osu.Framework.Platform;
using osu.Framework.Platform.Sdl;
using SDL2;

namespace osu.Framework.iOS
{
    public class IOSGraphicsBackend : PassthroughGraphicsBackend
    {
        public override void Initialise(IWindowBackend windowBackend)
        {
            base.Initialise(windowBackend);

            SDL.SDL_SysWMinfo info = new SDL.SDL_SysWMinfo();
            SDL.SDL_VERSION(out info.version);

            if (SDL.SDL_GetWindowWMInfo(SdlWindowHandle, ref info) == SDL.SDL_bool.SDL_TRUE)
                GLWrapper.DefaultFrameBuffer = (int)info.info.uikit.framebuffer;
        }
    }
}
