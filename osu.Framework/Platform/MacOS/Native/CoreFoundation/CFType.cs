// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Platform.MacOS.Native.CoreFoundation
{
    // ReSharper disable once InconsistentNaming
    public class CFType
    {
        internal const int CF_ARRAY = 18;
        internal const int CF_BOOLEAN = 21;
        internal const int CF_DATA = 19;
        internal const int CF_NUMBER = 22;
        internal const int CF_DICTIONARY = 17;
        internal const int CF_STRING = 7;

        public IntPtr Handle { get; }

        public CFType(IntPtr handle)
        {
            Handle = handle;
        }
    }
}
