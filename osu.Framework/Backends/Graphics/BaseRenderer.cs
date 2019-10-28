// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    public abstract class BaseRenderer : IRenderer
    {
        public abstract void ResetState(Vector2 size);
        public abstract void SetBlend(BlendingParameters blendingParameters);
        public abstract void SetDrawDepth(float drawDepth);

        public abstract MaskingInfo CurrentMaskingInfo { get; }
        public abstract RectangleI Viewport { get; }
        public abstract RectangleF Ortho { get; }
        public abstract Matrix4 ProjectionMatrix { get; }
        public abstract DepthInfo CurrentDepthInfo { get; }
        public abstract float BackbufferDrawDepth { get; }
        public abstract bool IsMaskingActive { get; }

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

        ~BaseRenderer()
        {
            Dispose(false);
        }

        #endregion
    }
}
