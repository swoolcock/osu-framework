// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    public class OsuTKRenderer : Renderer
    {
        public OsuTKRenderer(IGraphics graphics)
            : base(graphics)
        {
        }

        public override void Reset(Vector2 size) => GLWrapper.Reset(size);

        public override void Clear(ClearInfo clearInfo) => GLWrapper.Clear(clearInfo);

        public override void PushScissorState(bool enabled) => GLWrapper.PushScissorState(enabled);

        public override void PopScissorState() => GLWrapper.PopScissorState();

        public override void PushViewport(RectangleI viewport) => GLWrapper.PushViewport(viewport);

        public override void PopViewport() => GLWrapper.PopViewport();

        public override void PushOrtho(RectangleF ortho) => GLWrapper.PushOrtho(ortho);

        public override void PopOrtho() => GLWrapper.PopOrtho();

        public override void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false) =>
            GLWrapper.PushMaskingInfo(maskingInfo, overwritePreviousScissor);

        public override void PopMaskingInfo() => GLWrapper.PopMaskingInfo();

        public override void PushDepthInfo(DepthInfo depthInfo) => GLWrapper.PushDepthInfo(depthInfo);

        public override void PopDepthInfo() => GLWrapper.PopDepthInfo();
    }
}
