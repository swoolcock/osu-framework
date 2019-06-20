// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Framework.Backends.Audio
{
    public class BassAudioBackend : AudioBackend
    {
        public override void Initialise(IGameHost host)
        {
        }

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

        public override Track CreateTrack(Stream data, bool quick) => new TrackBass(data, quick);

        public override Sample CreateSample(byte[] data, ConcurrentQueue<Task> customPendingActions, int concurrency) => new SampleBass(data, customPendingActions, concurrency);

        public override SampleChannel CreateSampleChannel(Sample sample, Action<SampleChannel> onPlay) => new SampleChannelBass(sample, onPlay);
    }
}
