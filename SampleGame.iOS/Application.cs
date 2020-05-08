// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.iOS;
using SDL2;

namespace SampleGame.iOS
{
    public class Application
    {
        public static void Main(string[] args) => SDL.SDL_UIKitRunApp(0, IntPtr.Zero, gameMain);

        [ObjCRuntime.MonoPInvokeCallback(typeof(SDL.SDL_main_func))]
        private static int gameMain(int argc, IntPtr argv)
        {
            using (var host = new IOSHost())
                host.Run(new SampleGameGame());

            return 0;
        }
    }
}
