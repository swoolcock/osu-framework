// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Backends.Input.Veldrid.Keyboard;
using osu.Framework.Backends.Input.Veldrid.Mouse;
using osu.Framework.Backends.Window;
using osu.Framework.Extensions;
using osu.Framework.Input;
using osu.Framework.Input.Handlers;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;
using Veldrid;

namespace osu.Framework.Backends.Input
{
    public class VeldridInputBackend : InputBackend
    {
        internal InputSnapshot Snapshot;

        public override void Initialise(IGameHost host)
        {
            base.Initialise(host);

            if (!(host.Window is VeldridWindowBackend window))
                throw new Exception($"{nameof(VeldridInputBackend)} requires a corresponding {nameof(VeldridWindowBackend)}");

            window.Implementation.MouseDown += e =>
                OnMouseDown(new MouseButtonEventArgs((int)Snapshot.MousePosition.X, (int)Snapshot.MousePosition.Y, e.MouseButton.ToOsuTK(), true));

            window.Implementation.MouseUp += e =>
                OnMouseDown(new MouseButtonEventArgs((int)Snapshot.MousePosition.X, (int)Snapshot.MousePosition.Y, e.MouseButton.ToOsuTK(), false));

            window.Implementation.MouseWheel += e =>
                OnMouseScroll(new MouseWheelEventArgs((int)Snapshot.MousePosition.X, (int)Snapshot.MousePosition.Y, 0, (int)e.WheelDelta));

            window.Implementation.MouseMove += e =>
                OnMouseMove(new MouseMoveEventArgs((int)e.MousePosition.X, (int)e.MousePosition.Y, (int)(e.MousePosition.X - Snapshot.MousePosition.X), (int)(e.MousePosition.Y - Snapshot.MousePosition.Y)));
        }

        internal void TriggerKeypresses()
        {
            foreach (char c in Snapshot.KeyCharPresses)
                OnKeyPress(new KeyPressEventArgs(c));
        }

        public override IEnumerable<InputHandler> CreateInputHandlers() => new InputHandler[]
        {
            new VeldridKeyboardHandler(),
            new VeldridMouseHandler(),
        };

        public override ITextInputSource CreateTextInputSource() => Host.GetTextInput(); // new VeldridTextInputSource(Host.Input);
    }
}
