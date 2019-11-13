// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osuTK;
using osuTK.Input;
using Veldrid;

namespace osu.Framework.Backends.Input
{
    /// <summary>
    /// Provides input events and instantiates <see cref="InputHandler"/>s for those events.
    /// Currently uses osuTK's <see cref="KeyboardKeyEventArgs"/> and <see cref="MouseEventArgs"/>.
    /// </summary>
    public interface IInput : IBackend
    {
        #region Events

        event Action<KeyEvent> KeyDown;
        event Action<KeyEvent> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
        event EventHandler<MouseButtonEventArgs> MouseDown;
        event EventHandler<MouseButtonEventArgs> MouseUp;
        event EventHandler<MouseMoveEventArgs> MouseMove;
        event EventHandler<MouseWheelEventArgs> MouseScroll;

        #endregion

        IBindableList<InputHandler> AvailableInputHandlers { get; }

        void ResetInputHandlers();

        ITextInputSource CreateTextInputSource();
    }
}
