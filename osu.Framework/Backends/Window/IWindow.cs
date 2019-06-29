// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Drawing;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Input;
using osu.Framework.Platform;
using osuTK;

namespace osu.Framework.Backends.Window
{
    public interface IWindow : IBackend
    {
        #region Read-only Bindables

        IBindable<bool> Focused { get; }

        IBindable<bool> CursorInWindow { get; }

        IBindableList<WindowMode> SupportedWindowModes { get; }

        IBindable<bool> Visible { get; }

        #endregion

        #region Mutable Bindables

        Bindable<Rectangle> Bounds { get; }

        Bindable<Size> InternalSize { get; }

        Bindable<CursorState> CursorState { get; }

        // NOTE: this will use our own WindowState enum in future
        Bindable<WindowState> WindowState { get; }

        Bindable<WindowMode> WindowMode { get; }

        Bindable<ConfineMouseMode> ConfineMouseMode { get; }

        Bindable<string> Title { get; }

        #endregion

        #region Properties

        Size? MinimumSize { get; set; }

        Size? MaximumSize { get; set; }

        #endregion

        #region Events

        event Func<bool> CloseRequested;

        event Action Closed;

        event Action Update;

        #endregion

        #region Methods

        /// <summary>
        /// Triggers the backend's runloop.
        /// </summary>
        void Run();

        void Close();

        Point PointToClient(Point point);

        Point PointToScreen(Point point);

        #endregion
    }
}
