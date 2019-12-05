// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.InteropServices;

namespace osu.Framework.Platform.MacOS.Native.CoreFoundation
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct CFRange
    {
        public long Location;
        public long Length;

        public CFRange(long location, long length)
        {
            Location = location;
            Length = length;
        }
    }
}
