// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;

namespace osu.Framework.Platform.Driver.Audio
{
    public interface IAudioDriver : IDriver
    {
        Track CreateTrack(Stream data, bool quick = false);

        Sample CreateSample(byte[] data, ConcurrentQueue<Task> customPendingActions = null, int concurrency = Sample.DEFAULT_CONCURRENCY);

        SampleChannel CreateSampleChannel(Sample sample, Action<SampleChannel> onPlay);
    }
}
