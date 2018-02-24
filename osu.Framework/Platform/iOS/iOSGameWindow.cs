// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using OpenTK.Platform.iPhoneOS;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Framework.Platform.iOS
{
    public class iOSGameWindow : GameWindow
    {
        public iOSGameWindow(iOSPlatformGameView gameView)
            : base(new iOSPlatformGameWindow(gameView))
        {
        }

        public override void SetupWindow(FrameworkConfigManager config)
        {
            //throw new NotImplementedException();
        }
    }
}
