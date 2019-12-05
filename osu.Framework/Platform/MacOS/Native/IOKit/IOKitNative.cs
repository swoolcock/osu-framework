// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

// ReSharper disable InconsistentNaming

using System;
using System.Runtime.InteropServices;

namespace osu.Framework.Platform.MacOS.Native.IOKit
{
    public static class IOKitNative
    {
        internal const string LIB_IOKIT = "/System/Library/Frameworks/IOKit.framework/IOKit";

        #region IOHIDManager

        [DllImport(LIB_IOKIT)]
        public static extern int IOHIDManagerClose(IntPtr manager, uint optionBits);

        [DllImport(LIB_IOKIT)]
        public static extern IntPtr IOHIDManagerCreate(IntPtr allocator, uint optionBits);

        [DllImport(LIB_IOKIT)]
        public static extern IntPtr IOHIDManagerGetProperty(IntPtr manager, IntPtr key);

        [DllImport(LIB_IOKIT)]
        public static extern int IOHIDManagerOpen(IntPtr manager, uint optionBits);

        [DllImport(LIB_IOKIT)]
        public static extern void IOHIDManagerScheduleWithRunLoop(IntPtr manager, IntPtr runLoop, IntPtr runLoopMode);

        [DllImport(LIB_IOKIT)]
        public static extern void IOHIDManagerUnscheduleFromRunLoop(IntPtr manager, IntPtr runLoop, IntPtr runLoopMode);

        [DllImport(LIB_IOKIT)]
        public static extern void IOHIDManagerSetDeviceMatching(IntPtr manager, IntPtr matching);

        [DllImport(LIB_IOKIT)]
        public static extern IntPtr IOHIDManagerCopyDevices(IntPtr manager);

        #endregion

        #region IOHIDDevice

        [DllImport(LIB_IOKIT)]
        public static extern IntPtr IOHIDDeviceCreate(IntPtr allocator, IntPtr service);

        [DllImport(LIB_IOKIT)]
        public static extern IntPtr IOHIDDeviceGetProperty(IntPtr device, IntPtr key);

        #endregion
    }
}
