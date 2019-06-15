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

        private readonly BindableBool cursorInWindow = new BindableBool();
        public override IBindable<bool> CursorInWindow => cursorInWindow;

        private bool boundsChanging;
        private bool windowStateChanging;

        public OsuTKWindowDriver(IGameWindow implementation)
        {
            Implementation = implementation;

            Implementation.Closing += (sender, e) => e.Cancel = OnCloseRequested();
            Implementation.Closed += (sender, e) => OnClosed();
            Implementation.FocusedChanged += (sender, e) => focused.Value = Implementation.Focused;
            Implementation.Resize += implementation_MoveResize;
            Implementation.Move += implementation_MoveResize;
            Implementation.WindowStateChanged += implementation_WindowStateChanged;
            Implementation.MouseEnter += (sender, e) => cursorInWindow.Value = true;
            Implementation.MouseLeave += (sender, e) => cursorInWindow.Value = false;

            Bounds.ValueChanged += bounds_ValueChanged;
            WindowState.ValueChanged += windowState_ValueChanged;
            CursorState.ValueChanged += cursorState_ValueChanged;
        }

        public OsuTKWindowDriver(int width, int height)
            : this(new osuTK.GameWindow(width, height, new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, GraphicsMode.Default.Samples, GraphicsMode.Default.AccumulatorFormat, 3)))
        {
        }

        public override void Initialise(IDriverProvider provider)
        {
        }

        #region Event Handlers

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

        private void implementation_WindowStateChanged(object sender, EventArgs evt)
        {
            if (windowStateChanging)
                return;

            windowStateChanging = true;
            WindowState.Value = Implementation.WindowState;
            windowStateChanging = false;
        }

        private void windowState_ValueChanged(ValueChangedEvent<WindowState> evt)
        {
            if (windowStateChanging)
                return;

            windowStateChanging = true;
            Implementation.WindowState = evt.NewValue;
            windowStateChanging = false;
        }

        #endregion

        #region IDisposable

        private bool isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Implementation?.Dispose();
                }

                isDisposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
