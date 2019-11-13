// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Backends.Window.OsuTK;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    /// <summary>
    /// Concrete implementation of <see cref="IGraphics"/> that uses osuTK's OpenGL context and calls.
    /// </summary>
    public class OsuTKGraphicsBackend : OpenGLGraphicsBackend
    {
        private IGraphicsContext context;

        protected override void CreateContext(IGameHost host)
        {
            if (!(host.Window is OsuTKWindowBackend window))
                throw new BackendMismatchException(GetType(), typeof(OsuTKWindowBackend));

            if (window.Implementation is osuTK.GameWindow impl)
                context = impl.Context;

            MakeCurrent();
        }

        protected override void SetVSync(bool vsync)
        {
            if (Host.Window is OsuTKWindowBackend window && window.Implementation is osuTK.GameWindow gameWindow)
                gameWindow.VSync = vsync ? VSyncMode.On : VSyncMode.Off;
        }

        public override void MakeCurrent() => (Host.Window as OsuTKWindowBackend)?.Implementation.MakeCurrent();

        public override void SwapBuffers() => (Host.Window as OsuTKWindowBackend)?.Implementation.SwapBuffers();
    }
}
