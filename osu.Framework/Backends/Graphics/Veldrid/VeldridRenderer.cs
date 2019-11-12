// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Backends.Graphics.Veldrid
{
    public class VeldridRenderer : Renderer
    {
        public new VeldridGraphicsBackend Graphics => base.Graphics as VeldridGraphicsBackend;

        public VeldridRenderer(IGraphics graphics)
            : base(graphics)
        {
        }

        public override void Reset(Vector2 size) => throw new System.NotImplementedException();
        public override void Clear(ClearInfo clearInfo) => throw new System.NotImplementedException();
        public override void PushScissorState(bool enabled) => throw new System.NotImplementedException();
        public override void PopScissorState() => throw new System.NotImplementedException();
        public override void PushViewport(RectangleI viewport) => throw new System.NotImplementedException();
        public override void PopViewport() => throw new System.NotImplementedException();
        public override void PushOrtho(RectangleF ortho) => throw new System.NotImplementedException();
        public override void PopOrtho() => throw new System.NotImplementedException();
        public override void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false) => throw new System.NotImplementedException();
        public override void PopMaskingInfo() => throw new System.NotImplementedException();
        public override void PushDepthInfo(DepthInfo depthInfo) => throw new System.NotImplementedException();
        public override void PopDepthInfo() => throw new System.NotImplementedException();
    }
}
