// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Shaders;
using osu.Framework.IO.Stores;

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

        void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false);
    }
}
