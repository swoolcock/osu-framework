// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Backends.Input.OsuTK.Joystick;
using osu.Framework.Backends.Input.OsuTK.Keyboard;
using osu.Framework.Backends.Input.OsuTK.Mouse;
using osu.Framework.Backends.Window;
using osu.Framework.Extensions;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osu.Framework.Platform;

namespace osu.Framework.Backends.Input
{
    public class OsuTKInputBackend : InputBackend
    {
        public override void Initialise(IGameHost host)
        {
            base.Initialise(host);

            if (!(host.Window is OsuTKWindowBackend window))
                throw new Exception($"{nameof(OsuTKInputBackend)} requires a corresponding {nameof(OsuTKWindowBackend)}");

            window.Implementation.MouseDown += (_, e) => OnMouseDown(e);
            window.Implementation.MouseUp += (_, e) => OnMouseUp(e);
            window.Implementation.MouseWheel += (_, e) => OnMouseScroll(e);
            window.Implementation.MouseMove += (_, e) => OnMouseMove(e);
            window.Implementation.KeyDown += (_, e) => OnKeyDown(e.ToVeldrid());
            window.Implementation.KeyUp += (_, e) => OnKeyUp(e.ToVeldrid(false));
            window.Implementation.KeyPress += (_, e) => OnKeyPress(e);
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

        public override ITextInputSource CreateTextInputSource() => Host.GetTextInput();
    }
}
