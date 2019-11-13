// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Exposes graphics commands in an API-agnostic way.
    /// </summary>
    public interface IGraphics : IBackend
    {
        Bindable<bool> VerticalSync { get; }

        void MakeCurrent();
        void SwapBuffers();
    }
}
