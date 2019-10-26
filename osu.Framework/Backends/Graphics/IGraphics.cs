// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.IO.Stores;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Exposes graphics commands in an API-agnostic way.
    /// </summary>
    public interface IGraphics : IBackend
    {
        IShaderManager CreateShaderManager(ResourceStore<byte[]> store);

        void ResetState();
        void SetBlend(BlendingParameters blendingParameters);
        void SetDrawDepth(float drawDepth);

        MaskingInfo CurrentMaskingInfo { get; }
        RectangleI Viewport { get; }
        RectangleF Ortho { get; }
        Matrix4 ProjectionMatrix { get; }
        DepthInfo CurrentDepthInfo { get; }

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
    }
}
