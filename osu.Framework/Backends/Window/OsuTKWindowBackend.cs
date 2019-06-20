// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Drawing;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;
using osuTK.Platform;

namespace osu.Framework.Backends.Window
{
    public class OsuTKWindowBackend : WindowBackend
    {
        internal readonly IGameWindow Implementation;

        #region Read-only Bindables

        private readonly BindableBool focused = new BindableBool();
        public override IBindable<bool> Focused => focused;

        private readonly BindableBool cursorInWindow = new BindableBool();
        public override IBindable<bool> CursorInWindow => cursorInWindow;

        #endregion

        public OsuTKWindowBackend(IGameWindow implementation)
        {
            Implementation = implementation;

            Implementation.Closing += (sender, e) => e.Cancel = OnCloseRequested();
            Implementation.Closed += (sender, e) => OnClosed();
            Implementation.FocusedChanged += (sender, e) => focused.Value = Implementation.Focused;
            Implementation.Move += implementation_MoveResize;
            Implementation.Resize += implementation_MoveResize;
            Implementation.WindowStateChanged += implementation_WindowStateChanged;
            Implementation.MouseEnter += (sender, e) => cursorInWindow.Value = true;
            Implementation.MouseLeave += (sender, e) => cursorInWindow.Value = false;

            Bounds.ValueChanged += bounds_ValueChanged;
            InternalSize.ValueChanged += internalSize_ValueChanged;
            WindowState.ValueChanged += windowState_ValueChanged;
            CursorState.ValueChanged += cursorState_ValueChanged;
        }

        public OsuTKWindowBackend(int width, int height)
            : this(new osuTK.GameWindow(width, height, new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, GraphicsMode.Default.Samples, GraphicsMode.Default.AccumulatorFormat, 3)))
        {
        }

        public override void Initialise(IGameHost host)
        {
        }

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

        #region Event Handlers

        private bool boundsChangingFromEvent;
        private bool boundsChangingFromBindable;
        private bool windowStateChanging;

        private void implementation_MoveResize(object sender, EventArgs evt)
        {
            if (boundsChangingFromEvent)
                return;

            boundsChangingFromEvent = true;
            Bounds.Value = Implementation.Bounds;
            InternalSize.Value = Implementation.ClientSize;
            boundsChangingFromEvent = false;
        }

        private void bounds_ValueChanged(ValueChangedEvent<Rectangle> evt)
        {
            if (boundsChangingFromBindable)
                return;

            boundsChangingFromBindable = true;
            Implementation.Bounds = evt.NewValue;
            InternalSize.Value = Implementation.ClientSize;
            boundsChangingFromBindable = false;
        }

        private void internalSize_ValueChanged(ValueChangedEvent<Size> evt)
        {
            if (boundsChangingFromBindable)
                return;

            boundsChangingFromBindable = true;
            Implementation.ClientSize = evt.NewValue;
            Bounds.Value = Implementation.Bounds;
            boundsChangingFromBindable = false;
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
