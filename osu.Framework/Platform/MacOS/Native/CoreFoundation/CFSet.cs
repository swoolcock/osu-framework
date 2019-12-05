// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Platform.MacOS.Native.CoreFoundation
{
    public class CFSet : NativeObject
    {
        public CFSet(IntPtr handle)
            : base(handle)
        {
        }

        public long Count => CoreFoundationNative.CFSetGetCount(Handle);

        public void GetValues(out IntPtr[] values)
        {
            values = new IntPtr[Count];
            CoreFoundationNative.CFSetGetValues(Handle, values);
        }
    }
}
