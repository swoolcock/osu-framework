// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Drawing;
using osu.Framework.Bindables;

namespace osu.Framework.Platform.Driver.Window
{
    public interface IWindowDriver : IDriver
    {
        #region Bindables

        Bindable<Rectangle> Bounds { get; }

        IBindable<bool> Focused { get; }

        Bindable<CursorState> CursorState { get; }

        #endregion

        #region Events

        event Func<bool> CloseRequested;

        event Action Closed;

        #endregion
    }
}
