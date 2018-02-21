// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using ManagedBass;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using IOS::ObjCRuntime;

namespace osu.Framework.Audio.Sample
{
    internal class SampleBass : Sample, IBassAudio
    {
        private volatile int sampleId;

        private GCHandle pinnedData;
        private GCHandle pinnedInstance;

        public override bool IsLoaded => sampleId != 0;

        public SampleBass(byte[] data, ConcurrentQueue<Task> customPendingActions = null, int concurrency = DEFAULT_CONCURRENCY)
            : base(concurrency)
        {
            if (customPendingActions != null)
                PendingActions = customPendingActions;

            EnqueueAction(() =>
            {
                pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
                sampleId = Bass.SampleLoad(pinnedData.AddrOfPinnedObject(), 0, data.Length, PlaybackConcurrency, BassFlags.Default | BassFlags.SampleOverrideLongestPlaying);
                if (sampleId == 0)
                    pinnedData.Free();
                else
                {
                    pinnedInstance = GCHandle.Alloc(this, GCHandleType.Pinned);
                    Bass.ChannelSetSync(sampleId, SyncFlags.Free, 0, syncProcedure, GCHandle.ToIntPtr(pinnedInstance));
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            Bass.SampleFree(sampleId);

            if (pinnedData.IsAllocated)
                pinnedData.Free();
            
            if (pinnedInstance.IsAllocated)
                pinnedInstance.Free();
            
            base.Dispose(disposing);
        }

        void IBassAudio.UpdateDevice(int deviceIndex)
        {
            if (IsLoaded)
                // counter-intuitively, this is the correct API to use to migrate a sample to a new device.
                Bass.ChannelSetDevice(sampleId, deviceIndex);
        }

        public int CreateChannel() => Bass.SampleGetChannel(sampleId);

        [MonoPInvokeCallback(typeof(SyncProcedure))]
        private static void syncProcedure(int handle, int channel, int data, IntPtr user)
        {
            var gcHandle = GCHandle.FromIntPtr(user);
            SampleBass inst = (SampleBass)gcHandle.Target;
            if (inst.pinnedData.IsAllocated)
                inst.pinnedData.Free();
        }
    }
}
