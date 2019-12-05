// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Platform;
using osu.Framework;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Platform.MacOS.Native.IOKit;

namespace SampleGame.Desktop
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var manager = IOHIDManager.Create(IOHIDOptions.None);
            var dict = new Dictionary<string, string> { [IOHIDDevice.kIOHIDManufacturerKey] = "Logitech" };
            foreach (var device in manager.GetDevicesMatching(dict))
            {
                Console.WriteLine($"{device.Manufacturer} - {device.Product}");
            }

            // using (GameHost host = Host.GetSuitableHost(@"sample-game"))
            // using (Game game = new SampleGameGame())
            //     host.Run(game);
        }
    }
}
