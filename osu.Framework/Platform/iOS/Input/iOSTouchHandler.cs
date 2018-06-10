// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

extern alias IOS;

using System;
using osu.Framework.Input.Handlers;
using IOS::Foundation;
using IOS::UIKit;
using osu.Framework.Input;
using OpenTK;
using OpenTK.Input;

namespace osu.Framework.Platform.iOS.Input
{
    public class iOSTouchHandler : InputHandler
    {
        private readonly iOSPlatformGameView view;

        public iOSTouchHandler(iOSPlatformGameView view)
        {
            this.view = view;
            view.HandleTouches += handleTouches;
        }

        private void handleTouches(NSSet obj)
        {
            if (obj.Count == 1)
                handleUITouch((UITouch)obj.AnyObject);
            else
            {
                foreach (var t in obj)
                    handleUITouch((UITouch)t);
            }
        }

        private void handleUITouch(UITouch touch)
        {
            var location = touch.LocationInView(null);

            var basicState = new Framework.Input.MouseState();
            basicState.Position = new Vector2((float)location.X * view.Scale, (float)location.Y * view.Scale);

            switch (touch.Phase)
            {
                case UITouchPhase.Moved:
                case UITouchPhase.Began:
                    basicState.SetPressed(MouseButton.Left, true);
                    PendingStates.Enqueue(new InputState { Mouse = basicState });
                    break;
                case UITouchPhase.Cancelled:
                case UITouchPhase.Ended:
                    PendingStates.Enqueue(new InputState { Mouse = basicState });
                    break;
            }
        }

        public override bool IsActive => true;

        public override int Priority => 0;

        protected override void Dispose(bool disposing)
        {
            view.HandleTouches -= handleTouches;
            base.Dispose(disposing);
        }

        public override bool Initialize(GameHost host)
        {
            return true;
        }
    }
}
