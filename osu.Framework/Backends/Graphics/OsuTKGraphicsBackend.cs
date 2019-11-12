// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Window;
using osu.Framework.Platform;
using osuTK.Graphics;

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Concrete implementation of <see cref="IGraphicsBackend"/> that uses osuTK's OpenGL context and calls.
    /// </summary>
    public class OsuTKGraphicsBackend : OpenGLGraphicsBackend
    {
        private IGraphicsContext context;

        protected override void CreateContext(IGameHost host)
        {
            if (!(host.Window is OsuTKWindowBackend window))
                throw new Exception($"{nameof(OsuTKGraphicsBackend)} requires a corresponding {nameof(OsuTKWindowBackend)}");

            if (window.Implementation is osuTK.GameWindow impl)
                context = impl.Context;

            MakeCurrent();
        }

        public override void MakeCurrent() => (Host.Window as OsuTKWindowBackend)?.Implementation.MakeCurrent();

        public override void SwapBuffers() => (Host.Window as OsuTKWindowBackend)?.Implementation.SwapBuffers();
    }
}
