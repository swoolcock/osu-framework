// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using OpenTK.Graphics;
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

        internal override IGraphicsContext Context => throw new NotImplementedException();

        public override void SetupWindow(FrameworkConfigManager config)
        {
            //throw new NotImplementedException();
        }
    }
}
