// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Input.Handlers;
using osuTK;
using osuTK.Input;

namespace osu.Framework.Backends.Input
{
    /// <summary>
    /// Provides input events and instantiates <see cref="InputHandler"/>s for those events.
    /// Currently uses osuTK's <see cref="KeyboardKeyEventArgs"/> and <see cref="MouseEventArgs"/>.
    /// </summary>
    public interface IInput : IBackend
    {
        #region Events

        event EventHandler<KeyboardKeyEventArgs> KeyDown;
        event EventHandler<KeyboardKeyEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<MouseWheelEventArgs> MouseWheel;

        #endregion

        IBindableList<InputHandler> AvailableInputHandlers { get; }

        void ResetInputHandlers();
    }
}
