// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Graphics.Veldrid;
using osu.Framework.Backends.Window;
using osu.Framework.Platform;
using Veldrid;
using Veldrid.StartupUtilities;

namespace osu.Framework.Backends.Graphics
{
    public class VeldridGraphicsBackend : GraphicsBackend
    {
        internal GraphicsDevice Device;

        public override void Initialise(IGameHost host)
        {
            base.Initialise(host);

            if (!(host.Window is VeldridWindowBackend window))
                throw new Exception($"{nameof(VeldridGraphicsBackend)} requires a corresponding {nameof(VeldridWindowBackend)}");

            Device = VeldridStartup.CreateGraphicsDevice(window.Implementation);
        }

        public override IRenderer CreateRenderer() => new VeldridRenderer(this);
    }
}
