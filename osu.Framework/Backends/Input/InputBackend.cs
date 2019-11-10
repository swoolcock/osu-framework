// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Input.Handlers;
using osu.Framework.Platform;

namespace osu.Framework.Backends.Input
{
    /// <summary>
    /// Abstract implementation of <see cref="IInput"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class InputBackend : IInput
    {
        #region Events

        // public event EventHandler<KeyDownEvent> KeyDown;
        // public event EventHandler<KeyUpEvent> KeyUp;
        // public event EventHandler<KeyboardEvent> KeyPress;
        // public event EventHandler<MouseDownEvent> MouseDown;
        // public event EventHandler<MouseUpEvent> MouseUp;
        // public event EventHandler<MouseMoveEvent> MouseMove;
        // public event EventHandler<MouseScrollChangeEvent> MouseScroll;

        #endregion

        #region Event Invocation

        // protected virtual void OnKeyDown(KeyDownEvent args) => KeyDown?.Invoke(this, args);
        //
        // protected virtual void OnKeyUp(KeyUpEvent args) => KeyUp?.Invoke(this, args);
        //
        // protected virtual void OnKeyPress(KeyboardEvent args) => KeyPress?.Invoke(this, args);
        //
        // protected virtual void OnMouseDown(MouseDownEvent args) => MouseDown?.Invoke(this, args);
        //
        // protected virtual void OnMouseUp(MouseUpEvent args) => MouseUp?.Invoke(this, args);
        //
        // protected virtual void OnMouseMove(MouseMoveEvent args) => MouseMove?.Invoke(this, args);
        //
        // protected virtual void OnMouseScroll(MouseScrollChangeEvent args) => MouseScroll?.Invoke(this, args);

        #endregion

        protected IGameHost Host { get; private set; }

        public abstract IEnumerable<InputHandler> CreateInputHandlers();

        private readonly BindableList<InputHandler> availableInputHandlers = new BindableList<InputHandler>();

        public virtual IBindableList<InputHandler> AvailableInputHandlers => availableInputHandlers;

        public abstract void Configure(ConfigManager<FrameworkSetting> config);

        public virtual void Initialise(IGameHost host)
        {
            Host = host;
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

        ~InputBackend()
        {
            Dispose(false);
        }

        #endregion
    }
}
