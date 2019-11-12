// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    public interface IRenderer
    {
        void Reset(Vector2 size);
        void Clear(ClearInfo clearInfo);
        void PushScissorState(bool enabled);
        void PopScissorState();
        void PushViewport(RectangleI viewport);
        void PopViewport();
        void PushOrtho(RectangleF ortho);
        void PopOrtho();
        void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false);
        void PopMaskingInfo();
        void PushDepthInfo(DepthInfo depthInfo);
        void PopDepthInfo();
    }
}
