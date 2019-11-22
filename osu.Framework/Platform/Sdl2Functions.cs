// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid.Sdl2;

// ReSharper disable InconsistentNaming

namespace osu.Framework.Platform
{
    public unsafe class Sdl2Functions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SDL_GL_GetDrawableSize_t(SDL_Window window, int* w, int* h);

        private static readonly SDL_GL_GetDrawableSize_t s_glGetDrawableSize = Sdl2Native.LoadFunction<SDL_GL_GetDrawableSize_t>("SDL_GL_GetDrawableSize");

        public static Vector2 SDL_GL_GetDrawableSize(SDL_Window window)
        {
            int w, h;
            s_glGetDrawableSize(window, &w, &h);
            return new Vector2(w, h);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SDL_GL_GetSwapInterval_t();

        private static readonly SDL_GL_GetSwapInterval_t s_gl_getSwapInterval = Sdl2Native.LoadFunction<SDL_GL_GetSwapInterval_t>("SDL_GL_GetSwapInterval");

        public static int SDL_GL_GetSwapInterval() => s_gl_getSwapInterval();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SDL_GetNumVideoDisplays_t();

        private static readonly SDL_GetNumVideoDisplays_t s_getNumVideoDisplays = Sdl2Native.LoadFunction<SDL_GetNumVideoDisplays_t>("SDL_GetNumVideoDisplays");

        public static int SDL_GetNumVideoDisplays() => s_getNumVideoDisplays();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SDL_GetNumDisplayModes_t(int displayIndex);

        private static readonly SDL_GetNumDisplayModes_t s_getNumDisplayModes = Sdl2Native.LoadFunction<SDL_GetNumDisplayModes_t>("SDL_GetNumDisplayModes");

        public static int SDL_GetNumDisplayModes(int displayIndex) => s_getNumDisplayModes(displayIndex);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int SDL_GetDisplayMode_t(int displayIndex, int modeIndex, SDL_DisplayMode* mode);

        private static readonly SDL_GetDisplayMode_t s_getDisplayMode = Sdl2Native.LoadFunction<SDL_GetDisplayMode_t>("SDL_GetDisplayMode");

        public static DisplayMode? SDL_GetDisplayMode(int displayIndex, int modeIndex)
        {
            SDL_DisplayMode mode;
            mode.driverdata = IntPtr.Zero;
            if (s_getDisplayMode(displayIndex, modeIndex, &mode) != 0)
                return null;

            return new DisplayMode(modeIndex, mode.format, mode.width, mode.height, SDL_GetPixelFormatName(mode.format));
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate char* SDL_GetPixelFormatName_t(uint format);

        private static readonly SDL_GetPixelFormatName_t s_getPixelFormatName = Sdl2Native.LoadFunction<SDL_GetPixelFormatName_t>("SDL_GetPixelFormatName");

        public static string SDL_GetPixelFormatName(uint format) => Marshal.PtrToStringAnsi((IntPtr)s_getPixelFormatName(format));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_DisplayMode
    {
        public uint format;
        public int width;
        public int height;
        public IntPtr driverdata;
    }
}
