// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Abstract implementation of <see cref="IGraphics"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class GraphicsBackend : IGraphics
    {
        public abstract void Initialise(IGameHost host);
        public abstract void Configure(ConfigManager<FrameworkSetting> config);

        public abstract void ResetState();
        public abstract void SetBlend(BlendingParameters blendingParameters);
        public abstract void SetDrawDepth(float drawDepth);

        public abstract IShaderManager CreateShaderManager(ResourceStore<byte[]> store);

        public virtual MaskingInfo CurrentMaskingInfo { get; private set; }
        public virtual RectangleI Viewport { get; private set; }
        public virtual RectangleF Ortho { get; private set; }
        public virtual Matrix4 ProjectionMatrix { get; private set; }
        public virtual DepthInfo CurrentDepthInfo { get; private set; }
        public virtual bool IsMaskingActive { get; }

        public abstract void PushViewport(RectangleI viewport);
        public abstract void PopViewport();
        public abstract void PushOrtho(RectangleF ortho);
        public abstract void PopOrtho();
        public abstract void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false);
        public abstract void PopMaskingInfo();
        public abstract void PushDepthInfo(DepthInfo depthInfo);
        public abstract void PopDepthInfo();
        public abstract void Clear(ClearInfo clearInfo);
        public abstract void PushScissorState(bool enabled);
        public abstract void PopScissorState();

        #region IDisposable

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GraphicsBackend()
        {
            Dispose(false);
        }

        #endregion
    }
}
