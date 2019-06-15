// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Drawing;
using osu.Framework.Bindables;

namespace osu.Framework.Platform.Driver.Window
{
    /// <summary>
    /// Abstract implementation of <see cref="IWindowDriver"/> that will provide any base functionality required
    /// by driver subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class WindowDriver : IWindowDriver
    {
        public abstract void Initialise(IDriverProvider provider);

        #region Bindables

        public virtual Bindable<Rectangle> Bounds { get; } = new Bindable<Rectangle>();

        public virtual IBindable<bool> Focused { get; } = new Bindable<bool>();

        public virtual Bindable<CursorState> CursorState { get; } = new Bindable<CursorState>();

        #endregion

        #region Events

        public event Func<bool> CloseRequested;
        public event Action Closed;

        #endregion

        #region Event Invocation

        protected virtual bool OnCloseRequested() => CloseRequested?.Invoke() ?? false;

        protected virtual void OnClosed() => Closed?.Invoke();

        #endregion

        #region IDisposable

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WindowDriver()
        {
            Dispose(false);
        }

        #endregion
    }
}
