// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Input.Handlers;
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

        // event EventHandler<KeyDownEvent> KeyDown;
        // event EventHandler<KeyUpEvent> KeyUp;
        // event EventHandler<KeyboardEvent> KeyPress;
        // event EventHandler<MouseDownEvent> MouseDown;
        // event EventHandler<MouseUpEvent> MouseUp;
        // event EventHandler<MouseMoveEvent> MouseMove;
        // event EventHandler<MouseScrollChangeEvent> MouseScroll;

        #endregion

        IBindableList<InputHandler> AvailableInputHandlers { get; }

        void ResetInputHandlers();
    }
}
