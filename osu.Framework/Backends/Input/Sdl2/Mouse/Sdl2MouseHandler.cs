// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Input.OsuTK.Mouse;
using osu.Framework.Backends.Window.Sdl2;
using osu.Framework.Platform;
using osu.Framework.Threading;
using osuTK;

namespace osu.Framework.Backends.Input.Sdl2.Mouse
{
    internal class Sdl2MouseHandler : Sdl2MouseHandlerBase
    {
        private ScheduledDelegate scheduled;

        private Sdl2MouseState lastPollState;
        private Sdl2MouseState lastEventState;

        public override bool Initialize(IGameHost host)
        {
            base.Initialize(host);

            if (!(host.Window is Sdl2WindowBackend window))
                throw new Exception($"{nameof(Sdl2MouseHandler)} requires a corresponding {nameof(Sdl2WindowBackend)}");

            if (!(host.Input is Sdl2InputBackend input))
                throw new Exception($"{nameof(Sdl2MouseHandler)} requires a corresponding {nameof(Sdl2InputBackend)}");

            Enabled.BindValueChanged(e =>
            {
                if (e.NewValue)
                {
                    // window.Implementation.MouseMove += handleMouseEvent;
                    // window.Implementation.MouseDown += handleMouseEvent;
                    // window.Implementation.MouseUp += handleMouseEvent;
                    // window.Implementation.MouseWheel += handleMouseEvent;

                    // polling is used to keep a valid mouse position when we aren't receiving events.

                    // IReadOnlyList<MouseEvent> lastMouseEvents = null;
                    // int lastMouseEventsHash = 0;
                    // Vector2 lastPosition = Vector2.Zero;

                    host.InputThread.Scheduler.Add(scheduled = new ScheduledDelegate(delegate
                        {
                            // we should be getting events if the mouse is inside the window.
                            if (!host.Window.Visible.Value || host.Window.WindowState.Value == WindowState.Minimized) return;

                            // if (input.Snapshot.MouseEvents.Equals(lastMouseEvents)) return;
                            // int newHash = input.Snapshot.MouseEvents.GetHashCode();
                            //
                            // if (input.Snapshot.MousePosition.Equals(lastPosition) && newHash == lastMouseEventsHash) return;
                            //
                            // lastPosition = input.Snapshot.MousePosition;
                            //
                            // lastMouseEventsHash = newHash;

                            // var mapped = host.Window.PointToClient(new Point((int)input.Snapshot.MousePosition.X, (int)input.Snapshot.MousePosition.Y));

                            var newState = new Sdl2MouseState(input.Snapshot, window.Scale, host.IsActive.Value, null); //new Vector2(mapped.X, mapped.Y));
                            HandleState(newState, lastPollState, true);
                            lastPollState = newState;
                        }, 0, 1000.0 / 60));
                }
                else
                {
                    scheduled?.Cancel();

                    // window.Implementation.MouseMove -= handleMouseEvent;
                    // window.Implementation.MouseDown -= handleMouseEvent;
                    // window.Implementation.MouseUp -= handleMouseEvent;
                    // window.Implementation.MouseWheel -= handleMouseEvent;

                    lastPollState = null;
                    lastEventState = null;
                }
            }, true);

            return true;
        }

        private void handleMouseEvent(object sender, osuTK.Input.MouseEventArgs e)
        {
            if (!MouseInWindow.Value)
                return;

            if (e.Mouse.X < 0 || e.Mouse.Y < 0)
                // todo: investigate further why we are getting negative values from osuTK events
                // on windows when crossing centre screen boundaries (width/2 or height/2).
                return;

            var newState = new OsuTKEventMouseState(e.Mouse, Host.IsActive.Value, null);
            // HandleState(newState, lastEventState, true);
            // lastEventState = newState;
        }
    }
}
