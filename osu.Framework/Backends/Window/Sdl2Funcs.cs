// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.InteropServices;
using Veldrid.Sdl2;

// ReSharper disable InconsistentNaming

namespace osu.Framework.Backends.Window
{
    public unsafe class Sdl2Funcs
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SDL_GL_GetDrawableSize_t(SDL_Window window, int* w, int* h);

        private static readonly SDL_GL_GetDrawableSize_t s_glGetDrawableSize = Sdl2Native.LoadFunction<SDL_GL_GetDrawableSize_t>("SDL_GL_GetDrawableSize");
        public static void SDL_GL_GetDrawableSize(SDL_Window window, int* w, int* h) => s_glGetDrawableSize(window, w, h);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SDL_GetWindowBordersSize_t(SDL_Window window, int* top, int* left, int* bottom, int* right);

        private static readonly SDL_GetWindowBordersSize_t s_glGetWindowBordersSize = Sdl2Native.LoadFunction<SDL_GetWindowBordersSize_t>("SDL_GetWindowBordersSize");
        public static void SDL_GetWindowBordersSize(SDL_Window window, int* top, int* left, int* bottom, int* right) => s_glGetWindowBordersSize(window, top, left, bottom, right);
    }
}
