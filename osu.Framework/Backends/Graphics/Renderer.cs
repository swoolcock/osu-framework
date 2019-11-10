// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.Buffers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Vertices;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    public abstract class Renderer : IRenderer
    {
        public static IRenderer Shared { get; internal set; }

        internal IGraphics Graphics { get; }

        internal Renderer(IGraphics graphics)
        {
            Shared = this;
            Graphics = graphics;
        }

        public abstract void ScheduleDisposal(Action disposalAction);

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
        public abstract void FlushCurrentBatch();
        public abstract void SetActiveBatch(IVertexBatch batch);

        public abstract int CreateVertexBuffer<TVertex>(BufferUsage usage, uint size, int existingId = -1)
            where TVertex : struct, IVertex, IEquatable<TVertex>;

        public abstract int CreateIndexBuffer(uint size, int existingId = -1);

        public abstract void BindVertexBuffer<TVertex>(int id)
            where TVertex : struct, IVertex, IEquatable<TVertex>;

        public abstract void BindIndexBuffer(int id);

        public abstract void UpdateVertexBuffer<TVertex>(int offset, uint size, ref TVertex source)
            where TVertex : struct, IVertex, IEquatable<TVertex>;

        public abstract void UpdateIndexBuffer(BufferUsage usage, ushort[] indices, uint size);

        public abstract void DestroyBuffer(int id);

        public abstract void DrawIndices(BatchPrimitiveType type, int start, int count);

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

        ~Renderer()
        {
            Dispose(false);
        }

        #endregion
    }
}
