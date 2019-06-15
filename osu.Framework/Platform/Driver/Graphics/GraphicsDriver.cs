// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Platform.Driver.Graphics
{
    /// <summary>
    /// Abstract implementation of <see cref="IGraphicsDriver"/> that will provide any base functionality required
    /// by driver subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class GraphicsDriver : IGraphicsDriver
    {
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

        ~GraphicsDriver()
        {
            Dispose(false);
        }

        #endregion
    }
}
