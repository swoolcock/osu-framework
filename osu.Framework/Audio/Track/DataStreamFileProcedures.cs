// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using System;
using System.IO;
using System.Runtime.InteropServices;
using IOS::ObjCRuntime;
using ManagedBass;

namespace osu.Framework.Audio.Track
{
    internal class DataStreamFileProcedures
    {
        private byte[] readBuffer = new byte[32768];

        private readonly Stream dataStream;

        public FileProcedures BassProcedures => new FileProcedures
        {
            Close = ac_Close,
            Length = ac_Length,
            Read = ac_Read,
            Seek = ac_Seek
        };

        public DataStreamFileProcedures(Stream data)
        {
            dataStream = data;
        }

        [MonoPInvokeCallback(typeof(FileCloseProcedure))]
        private static void ac_Close(IntPtr user)
        {
            //manually handle closing of stream
        }

        [MonoPInvokeCallback(typeof(FileLengthProcedure))]
        private static long ac_Length(IntPtr user)
        {
            var handle = GCHandle.FromIntPtr(user);
            DataStreamFileProcedures inst = (DataStreamFileProcedures)handle.Target;

            if (inst.dataStream == null) return 0;

            try
            {
                return inst.dataStream.Length;
            }
            catch
            {
            }
        
            return 0;
        }

        [MonoPInvokeCallback(typeof(FileReadProcedure))]
        private static int ac_Read(IntPtr buffer, int length, IntPtr user)
        {
            var handle = GCHandle.FromIntPtr(user);
            DataStreamFileProcedures inst = (DataStreamFileProcedures)handle.Target;

            if (inst.dataStream == null) return 0;


            try
            {
                if (length > inst.readBuffer.Length)
                    inst.readBuffer = new byte[length];

                if (!inst.dataStream.CanRead)
                    return 0;

                int readBytes = inst.dataStream.Read(inst.readBuffer, 0, length);
                Marshal.Copy(inst.readBuffer, 0, buffer, readBytes);
                return readBytes;
            }
            catch
            {
            }

            return 0;
        }

        [MonoPInvokeCallback(typeof(FileSeekProcedure))]
        private static bool ac_Seek(long offset, IntPtr user)
        {
            var handle = GCHandle.FromIntPtr(user);
            DataStreamFileProcedures inst = (DataStreamFileProcedures)handle.Target;
            if (inst.dataStream == null) return false;

            try
            {
                return inst.dataStream.Seek(offset, SeekOrigin.Begin) == offset;
            }
            catch
            {
            }
            return false;
        }
    }
}
