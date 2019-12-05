// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

// ReSharper disable InconsistentNaming

using System;
using System.Runtime.InteropServices;

namespace osu.Framework.Platform.MacOS.Native.CoreFoundation
{
    public static class CoreFoundationNative
    {
        internal const string LIB_COREFOUNDATION = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        internal static readonly IntPtr Handle = NativeUtils.Load(LIB_COREFOUNDATION);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFRetain(IntPtr handle);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern void CFRelease(IntPtr handle);

        #region CFString

        [DllImport(LIB_COREFOUNDATION)]
        public static extern int CFStringGetLength(IntPtr handle);

        [DllImport(LIB_COREFOUNDATION, EntryPoint = "__CFStringMakeConstantString")]
        public static extern IntPtr CFStringMakeConstantString(string str);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFStringGetCharactersPtr(IntPtr handle);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFStringGetCharacters(IntPtr handle, CFRange range, IntPtr buffer);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern unsafe char* CFStringGetCStringPtr(IntPtr handle, uint encoding);

        #endregion

        #region CFDictionary

        public static readonly IntPtr kCFTypeDictionaryKeyCallBacks = NativeUtils.GetSymbol(Handle, "kCFTypeDictionaryKeyCallBacks");
        public static readonly IntPtr kCFTypeDictionaryValueCallBacks = NativeUtils.GetSymbol(Handle, "kCFTypeDictionaryValueCallBacks");

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFDictionaryCreate(IntPtr allocator, IntPtr[] keys, IntPtr[] vals, long len, IntPtr keyCallbacks, IntPtr valueCallbacks);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFDictionaryCreateMutable(IntPtr allocator, long capacity, IntPtr keyCallBacks, IntPtr valueCallBacks);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFDictionaryGetValue(IntPtr theDict, IntPtr key);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern long CFDictionaryGetCount(IntPtr theDict);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern void CFDictionaryGetKeysAndValues(IntPtr theDict, IntPtr[] keys, IntPtr[] values);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern void CFDictionarySetValue(IntPtr theDict, IntPtr key, IntPtr value);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern bool CFDictionaryContainsKey(IntPtr theDict, IntPtr key);

        #endregion

        #region CFSet

        public static readonly IntPtr kCFTypeSetCallBacks = NativeUtils.GetSymbol(Handle, "kCFTypeSetCallBacks");

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFSetCreate(IntPtr allocator, IntPtr[] values, long numValues, IntPtr callBacks);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern long CFSetGetCount(IntPtr theSet);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern IntPtr CFSetGetValue(IntPtr theSet, IntPtr value);

        [DllImport(LIB_COREFOUNDATION)]
        public static extern void CFSetGetValues(IntPtr theSet, IntPtr[] values);

        #endregion

        public static uint kCFStringEncodingUnicode = 0x0100;
    }
}
