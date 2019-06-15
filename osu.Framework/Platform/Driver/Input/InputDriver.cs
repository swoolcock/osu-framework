// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Platform.Driver.Input
{
    public abstract class InputDriver : IInputDriver
    {
        #region Events

        public event Action KeyDown;
        public event Action KeyUp;
        public event Action KeyPress;
        public event Action MouseDown;
        public event Action MouseUp;
        public event Action MouseMove;

        #endregion

        #region Event Invocation

        protected virtual void OnKeyDown() => KeyDown?.Invoke();

        protected virtual void OnKeyUp() => KeyUp?.Invoke();

        protected virtual void OnKeyPress() => KeyPress?.Invoke();

        protected virtual void OnMouseDown() => MouseDown?.Invoke();

        protected virtual void OnMouseUp() => MouseUp?.Invoke();

        protected virtual void OnMouseMove() => MouseMove?.Invoke();

        #endregion

        public abstract void Initialise(IDriverProvider provider);

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

        ~InputDriver()
        {
            Dispose(false);
        }

        #endregion
    }
}
