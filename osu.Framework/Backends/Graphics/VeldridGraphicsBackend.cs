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
using Veldrid;
using Veldrid.StartupUtilities;

namespace osu.Framework.Backends.Graphics
{
    public class VeldridGraphicsBackend : GraphicsBackend
    {
        internal GraphicsDevice Device;

        public override void Initialise(IGameHost host)
        {
            if (!(host.Window is VeldridWindowBackend window))
                throw new Exception($"{nameof(VeldridGraphicsBackend)} requires a corresponding {nameof(VeldridWindowBackend)}");

            Device = VeldridStartup.CreateGraphicsDevice(window.Implementation);
        }

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

        public override IShaderManager CreateShaderManager(ResourceStore<byte[]> store) => throw new NotImplementedException();

        public override void ResetState() => throw new NotImplementedException();

        public override void SetBlend(BlendingParameters blendingParameters) => throw new NotImplementedException();

        public override void SetDrawDepth(float drawDepth) => throw new NotImplementedException();

        public override void PushViewport(RectangleI viewport) => throw new NotImplementedException();

        public override void PopViewport() => throw new NotImplementedException();

        public override void PushOrtho(RectangleF ortho) => throw new NotImplementedException();

        public override void PopOrtho() => throw new NotImplementedException();

        public override void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false) => throw new NotImplementedException();

        public override void PopMaskingInfo() => throw new NotImplementedException();

        public override void PushDepthInfo(DepthInfo depthInfo) => throw new NotImplementedException();

        public override void PopDepthInfo() => throw new NotImplementedException();

        public override void Clear(ClearInfo clearInfo) => throw new NotImplementedException();

        public override void PushScissorState(bool enabled) => throw new NotImplementedException();

        public override void PopScissorState() => throw new NotImplementedException();
    }
}
