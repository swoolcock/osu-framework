// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Collections.Generic;
using OpenTK.Platform.iPhoneOS;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Keyboard;
using osu.Framework.Platform;
using osu.Framework.Platform.iOS.Input;
using osu.Framework.Platform.Windows;

namespace osu.Framework.Platform.iOS
{
    public class iOSGameHost : GameHost
    {
        private readonly iOSPlatformGameView gameView;

        public iOSGameHost(iOSPlatformGameView gameView)
        {
            this.gameView = gameView;

            Window = new iOSGameWindow(gameView);
        }

        public override ITextInputSource GetTextInput() => new iOSTextInput(gameView);

        protected override IEnumerable<InputHandler> CreateAvailableInputHandlers()
        {
            return new InputHandler[] { new iOSTouchHandler(gameView), new iOSKeyboardHandler(gameView) };
        }

        protected override Storage GetStorage(string baseName) => new WindowsStorage(baseName);
    }
}
