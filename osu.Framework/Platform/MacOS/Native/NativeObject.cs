// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Platform.MacOS.Native.CoreFoundation;

namespace osu.Framework.Platform.MacOS.Native
{
    public class NativeObject : IDisposable
    {
        public IntPtr Handle { get; protected set; }

        protected NativeObject()
        {
        }

        protected NativeObject(IntPtr handle)
        {
            Handle = handle;
        }

        public void Retain() => CoreFoundationNative.CFRetain(Handle);

        public void Release() => CoreFoundationNative.CFRelease(Handle);

        private void releaseUnmanagedResources()
        {
            if (Handle == IntPtr.Zero)
                return;

            Release();
            Handle = IntPtr.Zero;
        }

        protected virtual void Dispose(bool disposing) => releaseUnmanagedResources();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NativeObject()
        {
            Dispose(false);
        }
    }
}
