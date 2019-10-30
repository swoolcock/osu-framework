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
    public interface IRenderer
    {
        IGraphics Graphics { get; }

        void ScheduleDisposal(Action disposalAction);

        void ResetState(Vector2 size);
        void SetBlend(BlendingParameters blendingParameters);
        void SetDrawDepth(float drawDepth);

        MaskingInfo CurrentMaskingInfo { get; }
        RectangleI Viewport { get; }
        RectangleF Ortho { get; }
        Matrix4 ProjectionMatrix { get; }
        DepthInfo CurrentDepthInfo { get; }
        float BackbufferDrawDepth { get; }

        bool IsMaskingActive { get; }

        void PushViewport(RectangleI viewport);
        void PopViewport();
        void PushOrtho(RectangleF ortho);
        void PopOrtho();
        void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false);
        void PopMaskingInfo();
        void PushDepthInfo(DepthInfo depthInfo);
        void PopDepthInfo();
        void Clear(ClearInfo clearInfo);
        void PushScissorState(bool enabled);
        void PopScissorState();
        void FlushCurrentBatch();

        int CreateVertexBuffer<TVertex>(BufferUsage usage, uint size, int existingId = -1)
            where TVertex : struct, IVertex, IEquatable<TVertex>;

        int CreateIndexBuffer(uint size, int existingId = -1);

        void BindVertexBuffer<TVertex>(int id)
            where TVertex : struct, IVertex, IEquatable<TVertex>;

        void BindIndexBuffer(int id);

        void UpdateVertexBuffer<TVertex>(int offset, uint size, ref TVertex source)
            where TVertex : struct, IVertex, IEquatable<TVertex>;

        void UpdateIndexBuffer(BufferUsage usage, ushort[] indices, uint size);

        void DestroyBuffer(int id);

        void DrawIndices(BatchPrimitiveType type, int start, int count);
    }
}
