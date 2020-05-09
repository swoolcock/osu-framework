// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Keyboard;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Platform;
using UIKit;

namespace osu.Framework.iOS
{
    public class IOSHost : GameHost
    {
        protected override void SetupForRun()
        {
            base.SetupForRun();

            AllowScreenSuspension.BindValueChanged(allow =>
                    InputThread.Scheduler.Add(() => UIApplication.SharedApplication.IdleTimerDisabled = !allow.NewValue),
                true);
        }

        protected override void PerformExit(bool immediately)
        {
        }

        protected override IWindow CreateWindow() => new IOSWindow();

        public override void OpenFileExternally(string filename) => throw new NotImplementedException();

        public override void OpenUrlExternally(string url)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                NSUrl nsurl = NSUrl.FromString(url);
                if (UIApplication.SharedApplication.CanOpenUrl(nsurl))
                    UIApplication.SharedApplication.OpenUrl(nsurl, new NSDictionary(), null);
            });
        }

        public override Storage GetStorage(string path) => new IOSStorage(path, this);

        public override string UserStoragePath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        protected override IEnumerable<InputHandler> CreateAvailableInputHandlers() => new InputHandler[]
        {
            new KeyboardHandler(),
            new MouseHandler(),
        };

        public override ITextInputSource GetTextInput() => null;
    }
}
