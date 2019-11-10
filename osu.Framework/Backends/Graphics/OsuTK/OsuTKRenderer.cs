// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Vertices;
using osuTK;
using osuTK.Graphics.ES30;
using BufferUsage = osu.Framework.Graphics.Buffers.BufferUsage;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKRenderer : Renderer
    {
        public new OsuTKGraphicsBackend Graphics => (OsuTKGraphicsBackend)base.Graphics;

        public OsuTKRenderer(OsuTKGraphicsBackend graphics)
            : base(graphics)
        {
        }

        public override void ScheduleDisposal(Action disposalAction) => GLWrapper.ScheduleDisposal(disposalAction);

        public override MaskingInfo CurrentMaskingInfo => GLWrapper.CurrentMaskingInfo;
        public override RectangleI Viewport => GLWrapper.Viewport;
        public override RectangleF Ortho => GLWrapper.Ortho;
        public override Matrix4 ProjectionMatrix => GLWrapper.ProjectionMatrix;
        public override DepthInfo CurrentDepthInfo => GLWrapper.CurrentDepthInfo;
        public override float BackbufferDrawDepth => GLWrapper.BackbufferDrawDepth;
        public override bool IsMaskingActive => GLWrapper.IsMaskingActive;

        public override void ResetState(Vector2 size) => GLWrapper.Reset(size);
        public override void SetBlend(BlendingParameters blendingParameters) => GLWrapper.SetBlend(blendingParameters);
        public override void SetDrawDepth(float drawDepth) => GLWrapper.SetDrawDepth(drawDepth);
        public override void PushViewport(RectangleI viewport) => GLWrapper.PushViewport(viewport);
        public override void PopViewport() => GLWrapper.PopViewport();
        public override void PushOrtho(RectangleF ortho) => GLWrapper.PushOrtho(ortho);
        public override void PopOrtho() => GLWrapper.PopOrtho();
        public override void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false) => GLWrapper.PushMaskingInfo(maskingInfo, overwritePreviousScissor);
        public override void PopMaskingInfo() => GLWrapper.PopMaskingInfo();
        public override void PushDepthInfo(DepthInfo depthInfo) => GLWrapper.PushDepthInfo(depthInfo);
        public override void PopDepthInfo() => GLWrapper.PopDepthInfo();
        public override void Clear(ClearInfo clearInfo) => GLWrapper.Clear(clearInfo);
        public override void PushScissorState(bool enabled) => GLWrapper.PushScissorState(enabled);
        public override void PopScissorState() => GLWrapper.PopScissorState();
        public override void FlushCurrentBatch() => GLWrapper.FlushCurrentBatch();
        public override void SetActiveBatch(IVertexBatch batch) => GLWrapper.SetActiveBatch(batch);

        public override int CreateVertexBuffer<TVertex>(BufferUsage usage, uint size, int existingId = -1)
        {
            int id = GL.GenBuffer();

            BindVertexBuffer<TVertex>(id);

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)size, IntPtr.Zero, usage == BufferUsage.Dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);

            return id;
        }

        public override int CreateIndexBuffer(uint size, int existingId = -1) => existingId < 0 ? GL.GenBuffer() : existingId;

        public override void BindVertexBuffer<TVertex>(int id)
        {
            if (GLWrapper.BindBuffer(BufferTarget.ArrayBuffer, id))
                VertexUtils<TVertex>.Bind();
        }

        public override void BindIndexBuffer(int id) => GLWrapper.BindBuffer(BufferTarget.ElementArrayBuffer, id);

        public override void UpdateVertexBuffer<TVertex>(int offset, uint size, ref TVertex source) => GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, (IntPtr)size, ref source);

        public override void UpdateIndexBuffer(BufferUsage usage, ushort[] indices, uint size) => GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)size, indices, usage == BufferUsage.Dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);

        public override void DestroyBuffer(int id) => GL.DeleteBuffer(id);

        public override void DrawIndices(BatchPrimitiveType type, int start, int count) => GL.DrawElements(type.ToOpenGL(), count, DrawElementsType.UnsignedShort, (IntPtr)(start * sizeof(ushort)));
    }
}
