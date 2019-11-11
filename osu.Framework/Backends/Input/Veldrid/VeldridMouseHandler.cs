// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Numerics;
using osu.Framework.Backends.Input.OsuTK.Mouse;
using osu.Framework.Backends.Window;
using osu.Framework.Extensions;
using osu.Framework.Platform;
using osu.Framework.Threading;

namespace osu.Framework.Backends.Input.Veldrid
{
    internal class VeldridMouseHandler : VeldridMouseHandlerBase
    {
        private ScheduledDelegate scheduled;

        private VeldridMouseState lastPollState;
        private VeldridMouseState lastEventState;

        public override bool Initialize(IGameHost host)
        {
            base.Initialize(host);

            if (!(host.Window is VeldridWindowBackend window))
                throw new Exception($"{nameof(VeldridMouseHandler)} requires a corresponding {nameof(VeldridWindowBackend)}");

            if (!(host.Input is VeldridInputBackend input))
                throw new Exception($"{nameof(VeldridMouseHandler)} requires a corresponding {nameof(VeldridInputBackend)}");

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
                            // if (MouseInWindow.Value || !host.Window.Visible.Value || host.Window.WindowState.Value == WindowState.Minimized) return;

                            // if (input.Snapshot.MouseEvents.Equals(lastMouseEvents)) return;
                            // int newHash = input.Snapshot.MouseEvents.GetHashCode();
                            //
                            // if (input.Snapshot.MousePosition.Equals(lastPosition) && newHash == lastMouseEventsHash) return;
                            //
                            // lastPosition = input.Snapshot.MousePosition;
                            //
                            // lastMouseEventsHash = newHash;

                            // var mapped = host.Window.PointToClient(new Point(cursorState.X, cursorState.Y));

                            var newState = new VeldridMouseState(input.Snapshot, host.IsActive.Value, input.Snapshot.MousePosition.ToOsuTK()); // new Vector2(mapped.X, mapped.Y));
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
