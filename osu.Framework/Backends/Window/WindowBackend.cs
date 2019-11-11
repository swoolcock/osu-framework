// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Input;
using osu.Framework.Platform;
using osuTK;

namespace osu.Framework.Backends.Window
{
    /// <summary>
    /// Abstract implementation of <see cref="IWindow"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class WindowBackend : Backend, IWindow
    {
        #region Read-only Bindables

        public abstract IBindable<bool> Focused { get; }

        public abstract IBindable<bool> CursorInWindow { get; }

        private readonly BindableList<WindowMode> supportedWindowModes = new BindableList<WindowMode>();
        public virtual IBindableList<WindowMode> SupportedWindowModes => supportedWindowModes;

        public abstract IBindable<bool> Visible { get; }

        #endregion

        #region Mutable Bindables

        public virtual Bindable<Rectangle> Bounds { get; } = new Bindable<Rectangle>();

        public virtual Bindable<Size> InternalSize { get; } = new BindableSize();

        public virtual Bindable<CursorState> CursorState { get; } = new Bindable<CursorState>();

        public virtual Bindable<WindowState> WindowState { get; } = new Bindable<WindowState>();

        public virtual Bindable<WindowMode> WindowMode { get; } = new Bindable<WindowMode>();

        public virtual Bindable<ConfineMouseMode> ConfineMouseMode { get; } = new Bindable<ConfineMouseMode>();

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

        public event Action Update;

        #endregion

        #region Event Invocation

        protected virtual bool OnCloseRequested() => CloseRequested?.Invoke() ?? false;

        protected virtual void OnClosed() => Closed?.Invoke();

        protected virtual void OnUpdate() => Update?.Invoke();

        #endregion

        #region Methods

        public abstract void Run();

        public abstract void Close();

        public abstract Point PointToClient(Point point);

        public abstract Point PointToScreen(Point point);

        #endregion


    }
}
