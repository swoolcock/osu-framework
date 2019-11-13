// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Backends.Window.OsuTK;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.StateChanges;
using osu.Framework.Platform;
using osu.Framework.Statistics;
using osuTK.Input;
using KeyboardState = osu.Framework.Input.States.KeyboardState;

namespace osu.Framework.Backends.Input.OsuTK.Keyboard
{
    internal class OsuTKKeyboardHandler : InputHandler
    {
        public override bool IsActive => true;

        public override int Priority => 0;

        private TkKeyboardState lastEventState;
        private osuTK.Input.KeyboardState? lastRawState;

        public override bool Initialize(IGameHost host)
        {
            if (!(host.Window is OsuTKWindowBackend window))
                throw new Exception($"{nameof(OsuTKKeyboardHandler)} requires a corresponding {nameof(OsuTKWindowBackend)}");

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
                    lastRawState = null;
                    lastEventState = null;
                }
            }, true);

            return true;
        }

        private void handleKeyboardEvent(object sender, KeyboardKeyEventArgs e)
        {
            var rawState = e.Keyboard;

            if (lastRawState != null && rawState.Equals(lastRawState))
                return;

            lastRawState = rawState;

            var newState = new TkKeyboardState(rawState);

            PendingInputs.Enqueue(new KeyboardKeyInput(newState.Keys, lastEventState?.Keys));

            lastEventState = newState;

            FrameStatistics.Increment(StatisticsCounterType.KeyEvents);
        }

        private class TkKeyboardState : KeyboardState
        {
            private static readonly IEnumerable<Key> all_keys = Enum.GetValues(typeof(Key)).Cast<Key>();

            public TkKeyboardState(osuTK.Input.KeyboardState tkState)
            {
                if (tkState.IsAnyKeyDown)
                {
                    foreach (var key in all_keys)
                    {
                        if (tkState.IsKeyDown(key))
                        {
                            Keys.SetPressed(key, true);
                        }
                    }
                }
            }
        }
    }
}
