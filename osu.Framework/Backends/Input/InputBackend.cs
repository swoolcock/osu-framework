// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;
using Veldrid;

namespace osu.Framework.Backends.Input
{
    /// <summary>
    /// Abstract implementation of <see cref="IInput"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class InputBackend : Backend, IInput
    {
        #region Events

        public event Action<KeyEvent> KeyDown;
        public event Action<KeyEvent> KeyUp;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler<MouseButtonEventArgs> MouseDown;
        public event EventHandler<MouseButtonEventArgs> MouseUp;
        public event EventHandler<MouseMoveEventArgs> MouseMove;
        public event EventHandler<MouseWheelEventArgs> MouseScroll;

        #endregion

        #region Event Invocation

        protected virtual void OnKeyDown(KeyEvent args) => KeyDown?.Invoke(args);

        protected virtual void OnKeyUp(KeyEvent args) => KeyUp?.Invoke(args);

        protected virtual void OnKeyPress(KeyPressEventArgs args) => KeyPress?.Invoke(this, args);

        protected virtual void OnMouseDown(MouseButtonEventArgs args) => MouseDown?.Invoke(this, args);

        protected virtual void OnMouseUp(MouseButtonEventArgs args) => MouseUp?.Invoke(this, args);

        protected virtual void OnMouseMove(MouseMoveEventArgs args) => MouseMove?.Invoke(this, args);

        protected virtual void OnMouseScroll(MouseWheelEventArgs args) => MouseScroll?.Invoke(this, args);

        #endregion

        public abstract IEnumerable<InputHandler> CreateInputHandlers();

        private readonly BindableList<InputHandler> availableInputHandlers = new BindableList<InputHandler>();

        public virtual IBindableList<InputHandler> AvailableInputHandlers => availableInputHandlers;

        public override void Initialise(IGameHost host)
        {
            base.Initialise(host);

            ResetInputHandlers();
        }

        public void ResetInputHandlers()
        {
            foreach (var h in availableInputHandlers)
                h.Dispose();

            availableInputHandlers.Clear();

            var newHandlers = CreateInputHandlers();

            foreach (var handler in newHandlers)
            {
                if (!handler.Initialize(Host))
                {
                    handler.Enabled.Value = false;
                    break;
                }

                // TODO: (handler as IHasCursorSensitivity)?.Sensitivity.BindTo(cursorSensitivity);
            }

            availableInputHandlers.AddRange(newHandlers);
        }

        public abstract ITextInputSource CreateTextInputSource();
    }
}
