// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using osu.Framework.Platform.MacOS.Native.CoreFoundation;

namespace osu.Framework.Platform.MacOS.Native.IOKit
{
    // ReSharper disable once InconsistentNaming
    public class IOHIDManager : NativeObject
    {
        private List<IOHIDDevice> devices = new List<IOHIDDevice>();
        public IReadOnlyList<IOHIDDevice> Devices => devices;
        private IOHIDOptions options;

        internal IOHIDManager(IntPtr handle, IOHIDOptions options)
            : base(handle)
        {
            this.options = options;

            // IOHIDManagerRegisterInputReportCallback( hid, hidInputReportCallback, 0 );
            // IOHIDManagerScheduleWithRunLoop( hid, CFRunLoopGetMain(), kCFRunLoopDefaultMode );
        }

        public static IOHIDManager Create(IOHIDOptions options)
        {
            var handle = IOKitNative.IOHIDManagerCreate(IntPtr.Zero, (uint)options);
            return handle == IntPtr.Zero ? null : new IOHIDManager(handle, options);
        }

        public IEnumerable<IOHIDDevice> GetAllDevices() => GetDevicesMatching(null);

        public IEnumerable<IOHIDDevice> GetDevicesMatching(Dictionary<string, string> rules)
        {
            var dict = rules == null ? null : CFDictionary.FromDictionary(rules);
            IOKitNative.IOHIDManagerSetDeviceMatching(Handle, dict?.Handle ?? IntPtr.Zero);
            IOKitNative.IOHIDManagerOpen(Handle, (uint)options);

            var setHandle = IOKitNative.IOHIDManagerCopyDevices(Handle);
            var deviceSet = new CFSet(setHandle);
            deviceSet.GetValues(out var deviceHandles);
            return deviceHandles.Select(x => new IOHIDDevice(x));
        }

        private bool isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            isDisposed = true;

            // IOHIDManagerUnscheduleFromRunLoop( hid, CFRunLoopGetMain(), kCFRunLoopDefaultMode );

            devices.ForEach(x => x.Dispose());
            devices.Clear();

            IOKitNative.IOHIDManagerClose(Handle, (uint)options);

            base.Dispose(disposing);
        }
    }
}
