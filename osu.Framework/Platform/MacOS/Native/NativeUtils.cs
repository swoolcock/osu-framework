// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Runtime.InteropServices;

namespace osu.Framework.Platform.MacOS.Native
{
    public static class NativeUtils
    {
        internal const string LIB_DL = "libdl.dylib";
        internal const int RTLD_NOW = 2;

        [DllImport(LIB_DL)]
        private static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LIB_DL)]
        private static extern IntPtr dlopen(string fileName, int flags);

        public static IntPtr Load(string name)
        {
#if NETCOREAPP3_0
            if (NativeLibrary.TryLoad(name, out IntPtr handle))
                return handle;
#endif
            return dlopen(name, RTLD_NOW);
        }

        public static IntPtr GetSymbol(IntPtr handle, string name)
        {
#if NETCOREAPP3_0
            if (NativeLibrary.TryGetExport(handle, name, out IntPtr address))
                return address;
#endif
            return dlsym(handle, name);
        }

        public static IntPtr GetStringConstant(IntPtr handle, string symbol)
        {
            IntPtr ptr = GetSymbol(handle, symbol);
            return ptr == IntPtr.Zero ? IntPtr.Zero : Marshal.ReadIntPtr(ptr);
        }
    }
}
