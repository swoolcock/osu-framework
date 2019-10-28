// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Window;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;
using Veldrid;
using Veldrid.StartupUtilities;

namespace osu.Framework.Backends.Graphics
{
    internal class VeldridGraphicsBackend : GraphicsBackend
    {
        private GraphicsDevice device;

        public override void Initialise(IGameHost host)
        {
            if (!(host.Window is VeldridWindowBackend window))
                throw new Exception($"{nameof(VeldridGraphicsBackend)} requires a corresponding {nameof(VeldridWindowBackend)}");

            device = VeldridStartup.CreateGraphicsDevice(window.Implementation);
        }

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

        public override IShaderManager CreateShaderManager(ResourceStore<byte[]> store) => throw new NotImplementedException();

        public override IRenderer CreateRenderer() => throw new NotImplementedException();
    }
}
