// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using osu.Framework.Graphics.Video;

namespace osu.Framework.Backends.Video
{
    /// <summary>
    /// Abstract implementation of <see cref="IVideoDriver"/> that will provide any base functionality required
    /// by driver subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class VideoDriver : IVideoDriver
    {
        public abstract void Initialise(IDriverProvider provider);

        public abstract VideoDecoder CreateVideoDecoder(Stream stream);

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

        ~VideoDriver()
        {
            Dispose(false);
        }

        #endregion
    }
}
