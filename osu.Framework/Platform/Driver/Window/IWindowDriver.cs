// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Drawing;
using osu.Framework.Bindables;
using osuTK;

namespace osu.Framework.Platform.Driver.Window
{
    public interface IWindowDriver : IDriver
    {
        #region Read-only Bindables

        IBindable<bool> Focused { get; }

        IBindable<bool> CursorInWindow { get; }

        #endregion

        #region Mutable Bindables

        Bindable<Rectangle> Bounds { get; }

        Bindable<Size> InternalSize { get; }

        Bindable<CursorState> CursorState { get; }

        // NOTE: this will use our own WindowState enum in future
        Bindable<WindowState> WindowState { get; }

        Bindable<string> Title { get; }

        #endregion

        #region Properties

        Size? MinimumSize { get; set; }

        Size? MaximumSize { get; set; }

        #endregion

        #region Events

        event Func<bool> CloseRequested;

        event Action Closed;

        #endregion
    }
}
