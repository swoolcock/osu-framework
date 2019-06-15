// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Drawing;
using osu.Framework.Bindables;
using osuTK;
using osuTK.Graphics;
using osuTK.Platform;

namespace osu.Framework.Platform.Driver.Window
{
    public class OsuTKWindowDriver : WindowDriver
    {
        internal readonly IGameWindow Implementation;

        private readonly BindableBool focused = new BindableBool();
        public override IBindable<bool> Focused => focused;

        private bool boundsChanging;

        public OsuTKWindowDriver(IGameWindow implementation)
        {
            Implementation = implementation;
        }

        public OsuTKWindowDriver(int width, int height)
            : this(new osuTK.GameWindow(width, height, new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, GraphicsMode.Default.Samples, GraphicsMode.Default.AccumulatorFormat, 3)))
        {
        }

        public override void Initialise(IDriverProvider provider)
        {
            Implementation.Closing += (sender, e) => e.Cancel = OnCloseRequested();
            Implementation.Closed += (sender, e) => OnClosed();
            Implementation.FocusedChanged += (sender, e) => focused.Value = Implementation.Focused;
            Implementation.Resize += implementation_MoveResize;
            Implementation.Move += implementation_MoveResize;

            CursorState.ValueChanged += cursorState_ValueChanged;
            Bounds.ValueChanged += bounds_ValueChanged;
        }

        private void implementation_MoveResize(object sender, EventArgs evt)
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            Bounds.Value = Implementation.Bounds;
            boundsChanging = false;
        }

        private void bounds_ValueChanged(ValueChangedEvent<Rectangle> evt)
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            Implementation.Bounds = evt.NewValue;
            boundsChanging = false;
        }

        private void cursorState_ValueChanged(ValueChangedEvent<CursorState> evt)
        {
            Implementation.Cursor = evt.NewValue.HasFlag(Platform.CursorState.Hidden) ? MouseCursor.Empty : MouseCursor.Default;

            try
            {
                Implementation.CursorGrabbed = evt.NewValue.HasFlag(Platform.CursorState.Confined);
            }
            catch
            {
                // may not be supported by platform.
            }
        }
    }
}
