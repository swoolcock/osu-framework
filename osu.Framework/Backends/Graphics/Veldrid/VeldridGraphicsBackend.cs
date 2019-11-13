// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Window.Sdl2;
using osu.Framework.Platform;
using Veldrid;
using Veldrid.StartupUtilities;

namespace osu.Framework.Backends.Graphics.Veldrid
{
    public class VeldridGraphicsBackend : GraphicsBackend
    {
        internal GraphicsDevice Device;

        public override void Initialise(IGameHost host)
        {
            base.Initialise(host);

            if (!(host.Window is Sdl2WindowBackend window))
                throw new Exception($"{nameof(VeldridGraphicsBackend)} requires a corresponding {nameof(Sdl2WindowBackend)}");

            Device = VeldridStartup.CreateGraphicsDevice(window.Implementation);
        }

        protected override void SetVSync(bool vsync) => Device.SyncToVerticalBlank = vsync;

        public override IRenderer CreateRenderer() => new VeldridRenderer(this);
    }
}
