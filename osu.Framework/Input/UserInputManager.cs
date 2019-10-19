// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.StateChanges;
using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Platform;
using osuTK;
using osuTK.Input;

namespace osu.Framework.Input
{
    public class UserInputManager : PassThroughInputManager
    {
        protected override IEnumerable<InputHandler> InputHandlers => Host.Input.AvailableInputHandlers;

        protected override bool HandleHoverEvents => Host.Window.CursorInWindow.Value;

        protected internal override bool ShouldBeAlive => true;

        protected internal UserInputManager()
        {
            // UserInputManager is at the very top of the draw hierarchy, so it has no parnt updating its IsAlive state
            IsAlive = true;
            UseParentInput = false;
        }

        public override void HandleInputStateChange(InputStateChangeEvent inputStateChange)
        {
            switch (inputStateChange)
            {
                case MousePositionChangeEvent mousePositionChange:
                    var mouse = mousePositionChange.State.Mouse;
                    // confine cursor
                    if (Host.Window != null && Host.Window.CursorState.Value.HasFlag(CursorState.Confined))
                        mouse.Position = Vector2.Clamp(mouse.Position, Vector2.Zero, new Vector2(Host.Window.InternalSize.Value.Width, Host.Window.InternalSize.Value.Height));
                    break;

                case ButtonStateChangeEvent<MouseButton> buttonChange:
                    if (buttonChange.Kind == ButtonStateChangeKind.Pressed && Host.Window?.CursorInWindow.Value == false)
                        return;

                    break;

                case MouseScrollChangeEvent _:
                    if (Host.Window?.CursorInWindow.Value == false)
                        return;

                    break;
            }

            base.HandleInputStateChange(inputStateChange);
        }
    }
}
