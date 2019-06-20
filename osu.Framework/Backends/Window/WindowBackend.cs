// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osuTK;

namespace osu.Framework.Backends.Window
{
    /// <summary>
    /// Abstract implementation of <see cref="IWindow"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class WindowBackend : IWindow
    {
        public abstract void Initialise(IGameHost host);
        public abstract void Configure(ConfigManager<FrameworkSetting> config);

        #region Read-only Bindables

        public abstract IBindable<bool> Focused { get; }

        public abstract IBindable<bool> CursorInWindow { get; }

        private readonly BindableList<WindowMode> supportedWindowModes = new BindableList<WindowMode>();
        public virtual IBindableList<WindowMode> SupportedWindowModes => supportedWindowModes;

        #endregion

        #region Mutable Bindables

        public virtual Bindable<Rectangle> Bounds { get; } = new Bindable<Rectangle>();

        public virtual Bindable<Size> InternalSize { get; } = new BindableSize();

        public virtual Bindable<CursorState> CursorState { get; } = new Bindable<CursorState>();

        public virtual Bindable<WindowState> WindowState { get; } = new Bindable<WindowState>();

        public virtual Bindable<WindowMode> WindowMode { get; } = new Bindable<WindowMode>();

        public virtual Bindable<string> Title { get; } = new Bindable<string>();

        #endregion

        #region Defaults

        protected abstract IEnumerable<WindowMode> DefaultSupportedWindowModes { get; }

        protected virtual WindowMode DefaultWindowMode => SupportedWindowModes.First();

        #endregion

        protected WindowBackend()
        {
            supportedWindowModes.AddRange(DefaultSupportedWindowModes);
            WindowMode.Default = DefaultWindowMode;
            WindowMode.SetDefault();
        }

        #region Properties

        public Size? MinimumSize { get; set; }

        public Size? MaximumSize { get; set; }

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

        ~WindowBackend()
        {
            Dispose(false);
        }

        #endregion
    }
}
