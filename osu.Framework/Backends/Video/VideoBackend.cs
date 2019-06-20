// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Video;
using osu.Framework.Platform;

namespace osu.Framework.Backends.Video
{
    /// <summary>
    /// Abstract implementation of <see cref="IVideo"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class VideoBackend : IVideo
    {
        public abstract void Initialise(IGameHost host);
        public abstract void Configure(ConfigManager<FrameworkSetting> config);

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
