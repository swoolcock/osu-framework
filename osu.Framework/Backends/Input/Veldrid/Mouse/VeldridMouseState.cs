// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions;
using osu.Framework.Input.States;
using osuTK;
using Veldrid;

namespace osu.Framework.Backends.Input.Veldrid.Mouse
{
    internal class VeldridMouseState : MouseState
    {
        public readonly bool WasActive;
        public readonly bool HasPreciseScroll;
        public InputSnapshot Snapshot;

        public VeldridMouseState(InputSnapshot snapshot, float scale, bool active, Vector2? mappedPosition)
        {
            WasActive = active;

            Snapshot = snapshot;

            // While not focused, let's silently ignore everything but position.
            if (active)
            {
                addIfPressed(MouseButton.Left);
                addIfPressed(MouseButton.Middle);
                addIfPressed(MouseButton.Right);
                addIfPressed(MouseButton.Button1);
                addIfPressed(MouseButton.Button2);
            }

            Scroll = new Vector2(0, snapshot.WheelDelta);
            HasPreciseScroll = false; // FIXME: tkState.Flags.HasFlag(MouseStateFlags.HasPreciseScroll);
            Position = new Vector2(mappedPosition?.X ?? snapshot.MousePosition.X * scale, mappedPosition?.Y ?? snapshot.MousePosition.Y * scale);
        }

        private void addIfPressed(MouseButton button)
        {
            if (Snapshot.IsMouseDown(button))
                SetPressed(button.ToOsuTK(), true);
        }
    }
}
