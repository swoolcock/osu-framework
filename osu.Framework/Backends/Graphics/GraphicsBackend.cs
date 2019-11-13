// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Platform;

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Abstract implementation of <see cref="IGraphics"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class GraphicsBackend : Backend, IGraphics
    {
        public virtual Bindable<bool> VerticalSync { get; } = new Bindable<bool>();

        public override void Initialise(IGameHost host)
        {
            base.Initialise(host);

            VerticalSync.ValueChanged += e => host.DrawThread.Scheduler.Add(() => SetVSync(e.NewValue));
        }

        protected abstract void SetVSync(bool vsync);

        public abstract IRenderer CreateRenderer();

        public virtual void MakeCurrent()
        {
        }

        public virtual void SwapBuffers()
        {
        }
    }
}
