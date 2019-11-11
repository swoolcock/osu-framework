// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Backends.Input.OsuTK.Joystick;
using osu.Framework.Backends.Input.OsuTK.Keyboard;
using osu.Framework.Backends.Input.OsuTK.Mouse;
using osu.Framework.Input.Handlers;

namespace osu.Framework.Backends.Input
{
    public class OsuTKInputBackend : InputBackend
    {
        public override IEnumerable<InputHandler> CreateInputHandlers()
        {
            var defaultEnabled = new InputHandler[]
            {
                new OsuTKMouseHandler(),
                new OsuTKKeyboardHandler(),
                new OsuTKJoystickHandler(),
            };

            var defaultDisabled = new InputHandler[]
            {
                new OsuTKRawMouseHandler(),
            };

            foreach (var h in defaultDisabled)
                h.Enabled.Value = false;

            // NOTE: at this point we would connect the handlers to the backend events

            return defaultEnabled.Concat(defaultDisabled);
        }
    }
}
