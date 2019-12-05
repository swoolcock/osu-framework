// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;

namespace osu.Framework.Platform.MacOS.Native.CoreFoundation
{
    public class CFDictionary : NativeObject
    {
        public CFDictionary(IntPtr handle)
            : base(handle)
        {
        }

        public static CFDictionary FromDictionary(Dictionary<string, string> dict)
        {
            var keys = dict.Keys.Select(x => new CFString(x));
            var values = dict.Values.Select(x => new CFString(x));
            return FromObjectsAndKeys(keys.ToArray<NativeObject>(), values.ToArray<NativeObject>());
        }

        public static CFDictionary FromObjectAndKey(NativeObject obj, NativeObject key) =>
            new CFDictionary(CoreFoundationNative.CFDictionaryCreate(IntPtr.Zero,
                new IntPtr[] { key.Handle }, new IntPtr[] { obj.Handle }, 1,
                CoreFoundationNative.kCFTypeDictionaryKeyCallBacks, CoreFoundationNative.kCFTypeDictionaryValueCallBacks));

        public static CFDictionary FromObjectsAndKeys(NativeObject[] objects, NativeObject[] keys)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            if (objects.Length != keys.Length)
                throw new ArgumentException($"Lengths of {nameof(objects)} and {nameof(keys)} must be equal.");

            IntPtr[] objectHandles = objects.Select(x => x.Handle).ToArray();
            IntPtr[] keyHandles = keys.Select(x => x.Handle).ToArray();

            return new CFDictionary(CoreFoundationNative.CFDictionaryCreate(IntPtr.Zero,
                keyHandles, objectHandles, keys.Length,
                CoreFoundationNative.kCFTypeDictionaryKeyCallBacks, CoreFoundationNative.kCFTypeDictionaryValueCallBacks));
        }

        public long Count => CoreFoundationNative.CFDictionaryGetCount(Handle);

        public void GetKeysAndValues(out IntPtr[] keys, out IntPtr[] values)
        {
            long count = Count;
            keys = new IntPtr[count];
            values = new IntPtr[count];
            CoreFoundationNative.CFDictionaryGetKeysAndValues(Handle, keys, values);
        }

        private IntPtr getValueHandle(string key)
        {
            using (CFString cfk = new CFString(key))
                return CoreFoundationNative.CFDictionaryGetValue(Handle, cfk.Handle);
        }

        public string GetStringValue(string key) => CFString.GetString(getValueHandle(key));
    }
}
