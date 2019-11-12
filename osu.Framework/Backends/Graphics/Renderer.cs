// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

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
    }
}
