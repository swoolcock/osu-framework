// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Platform.MacOS.Native.IOKit
{
    public readonly struct DeviceMouse
    {
        public readonly IntPtr Handle;
        public readonly string Name;
        public readonly int Index;

        public DeviceMouse(IntPtr handle, string name, int index)
        {
            Handle = handle;
            Name = name;
            Index = index;
        }
    }
}
