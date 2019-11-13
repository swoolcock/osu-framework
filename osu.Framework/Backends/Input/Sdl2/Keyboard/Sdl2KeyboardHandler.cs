// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Window.Sdl2;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.StateChanges;
using osu.Framework.Input.States;
using osu.Framework.Platform;
using osu.Framework.Statistics;
using Veldrid;
using Key = osuTK.Input.Key;

namespace osu.Framework.Backends.Input.Sdl2.Keyboard
{
    internal class Sdl2KeyboardHandler : InputHandler
    {
        private readonly KeyboardState lastKeyboardState = new KeyboardState();
        private readonly KeyboardState thisKeyboardState = new KeyboardState();

        public override bool IsActive => true;

        public override int Priority => 0;

        public override bool Initialize(IGameHost host)
        {
            if (!(host.Window is Sdl2WindowBackend window))
                throw new Exception($"{nameof(Sdl2KeyboardHandler)} requires a corresponding {nameof(Sdl2WindowBackend)}");

            Enabled.BindValueChanged(e =>
            {
                if (e.NewValue)
                {
                    window.Implementation.KeyDown += handleKeyboardEvent;
                    window.Implementation.KeyUp += handleKeyboardEvent;
                }
                else
                {
                    window.Implementation.KeyDown -= handleKeyboardEvent;
                    window.Implementation.KeyUp -= handleKeyboardEvent;
                }
            }, true);

            return true;
        }

        private void handleKeyboardEvent(KeyEvent keyEvent)
        {
            thisKeyboardState.Keys.SetPressed((Key)keyEvent.Key, keyEvent.Down);
            PendingInputs.Enqueue(new KeyboardKeyInput(thisKeyboardState.Keys, lastKeyboardState.Keys));
            lastKeyboardState.Keys.SetPressed((Key)keyEvent.Key, keyEvent.Down);
            FrameStatistics.Increment(StatisticsCounterType.KeyEvents);
        }
    }
}
