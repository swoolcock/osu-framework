// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Window;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.StateChanges;
using osu.Framework.Platform;
using osu.Framework.Statistics;
using Veldrid;
using Key = osuTK.Input.Key;

namespace osu.Framework.Backends.Input.Veldrid.Keyboard
{
    internal class VeldridKeyboardHandler : InputHandler
    {
        public override bool IsActive => true;

        public override int Priority => 0;

        public override bool Initialize(IGameHost host)
        {
            if (!(host.Window is VeldridWindowBackend window))
                throw new Exception($"{nameof(VeldridKeyboardHandler)} requires a corresponding {nameof(VeldridWindowBackend)}");

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
            PendingInputs.Enqueue(new KeyboardKeyInput((Key)keyEvent.Key, keyEvent.Down));

            FrameStatistics.Increment(StatisticsCounterType.KeyEvents);
        }
    }
}
