// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKRenderer : BaseRenderer
    {
        private OsuTKGraphicsBackend graphics;

        public OsuTKRenderer(OsuTKGraphicsBackend graphics)
        {
            this.graphics = graphics;
        }

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
    }
}
