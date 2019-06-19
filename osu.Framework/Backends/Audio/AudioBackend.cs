// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;

namespace osu.Framework.Backends.Audio
{
    /// <summary>
    /// Abstract implementation of <see cref="IAudioBackend"/> that will provide any base functionality required
    /// by backend subclasses that should not be exposed via the interface.
    /// </summary>
    public abstract class AudioBackend : IAudioBackend
    {
        #region IAudioBackend

        public abstract void Initialise(IBackendProvider provider);

        public abstract Track CreateTrack(Stream data, bool quick);

        public abstract Sample CreateSample(byte[] data, ConcurrentQueue<Task> customPendingActions, int concurrency);

        public abstract SampleChannel CreateSampleChannel(Sample sample, Action<SampleChannel> onPlay);

        #endregion

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

        ~AudioBackend()
        {
            Dispose(false);
        }

        #endregion
    }
}
