// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Backends.Window;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.Handlers.Joystick;
using osu.Framework.Input.Handlers.Keyboard;
using osu.Framework.Input.Handlers.Mouse;

namespace osu.Framework.Backends.Input
{
    public class OsuTKInputBackend : InputBackend
    {
        public override void Initialise(IBackendProvider provider)
        {
            if (!(provider.Window is OsuTKWindowBackend window))
                throw new Exception($"{nameof(OsuTKInputBackend)} requires a corresponding {nameof(OsuTKWindowBackend)}");

            // osuTK input events are triggered through the osuTK.GameWindow implementation
            window.Implementation.KeyDown += (sender, e) => OnKeyDown();
            window.Implementation.KeyUp += (sender, e) => OnKeyUp();
            window.Implementation.KeyPress += (sender, e) => OnKeyPress();
            window.Implementation.MouseDown += (sender, e) => OnMouseDown();
            window.Implementation.MouseUp += (sender, e) => OnMouseUp();
            window.Implementation.MouseMove += (sender, e) => OnMouseMove();
        }

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
