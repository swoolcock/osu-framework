// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using osu.Framework.Graphics.Video;

namespace osu.Framework.Backends.Video
{
    /// <summary>
    /// Abstract implementation of <see cref="IVideo"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class VideoBackend : IVideo
    {
        public abstract void Initialise(IBackendProvider provider);

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

        ~VideoBackend()
        {
            Dispose(false);
        }

        #endregion
    }
}
