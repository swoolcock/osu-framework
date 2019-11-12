// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Abstract implementation of <see cref="IGraphics"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class GraphicsBackend : Backend, IGraphics
    {
        public virtual void MakeCurrent()
        {
        }

        public virtual void SwapBuffers()
        {
        }
    }
}
