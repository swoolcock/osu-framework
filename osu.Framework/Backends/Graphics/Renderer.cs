// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    public abstract class Renderer : IRenderer
    {
        public static IRenderer Shared { get; private set; }

        protected IGraphics Graphics { get; }

        protected Renderer(IGraphics graphics)
        {
            Shared = this;
            Graphics = graphics;
        }

        public abstract void Reset(Vector2 size);
        public abstract void Clear(ClearInfo clearInfo);
        public abstract void PushScissorState(bool enabled);
        public abstract void PopScissorState();
        public abstract void PushViewport(RectangleI viewport);
        public abstract void PopViewport();
        public abstract void PushOrtho(RectangleF ortho);
        public abstract void PopOrtho();
        public abstract void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false);
        public abstract void PopMaskingInfo();
        public abstract void PushDepthInfo(DepthInfo depthInfo);
        public abstract void PopDepthInfo();
    }
}
