// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Runtime.InteropServices;

namespace osu.Framework.Platform.MacOS.Native.CoreFoundation
{
    // ReSharper disable once InconsistentNaming
    public class CFString : NativeObject
    {
        internal string Source;

        public CFString(IntPtr handle)
            : base(handle)
        {
        }

        public CFString(string str)
        {
            Handle = CoreFoundationNative.CFStringMakeConstantString(str);
            Source = str;
        }

        public static unsafe string GetString(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            int length = CoreFoundationNative.CFStringGetLength(handle);
            IntPtr ptr = CoreFoundationNative.CFStringGetCharactersPtr(handle);
            IntPtr buffer = IntPtr.Zero;

            if (ptr == IntPtr.Zero)
            {
                CFRange range = new CFRange(0, length);
                buffer = Marshal.AllocCoTaskMem(length * 2);
                CoreFoundationNative.CFStringGetCharacters(handle, range, buffer);
                ptr = buffer;
            }

            string str = new string((char*)ptr, 0, length);

            if (buffer != IntPtr.Zero)
                Marshal.FreeCoTaskMem(buffer);

            return str;
        }

        public override string ToString() => Source ??= GetString(Handle);

        // public static implicit operator string(CFString x) => x.Source ?? (x.Source = GetString(x.Handle));

        public int Length => Source?.Length ?? CoreFoundationNative.CFStringGetLength(Handle);
    }
}
