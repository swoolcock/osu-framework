// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Drawing;
using osu.Framework.Backends.Input;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Point = System.Drawing.Point;

// ReSharper disable InconsistentNaming

namespace osu.Framework.Backends.Window
{
    public class VeldridWindowBackend : WindowBackend
    {
        internal readonly Sdl2Window Implementation;
        internal IntPtr SdlWindow;

        #region Read-only Bindables

        private readonly BindableBool focused = new BindableBool();
        public override IBindable<bool> Focused => focused;

        private readonly BindableBool cursorInWindow = new BindableBool();
        public override IBindable<bool> CursorInWindow => cursorInWindow;

        private readonly BindableBool visible = new BindableBool();
        public override IBindable<bool> Visible => visible;

        #endregion

        protected override IEnumerable<WindowMode> DefaultSupportedWindowModes => new[]
        {
            Configuration.WindowMode.Windowed,
            Configuration.WindowMode.Borderless,
            Configuration.WindowMode.Fullscreen,
        };

        public VeldridWindowBackend()
        {
            WindowCreateInfo windowCi = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Test"
            };
            Implementation = VeldridStartup.CreateWindow(ref windowCi);
            SdlWindow = Implementation.SdlWindowHandle;

            Implementation.Resized += implementation_Resized;
            Implementation.Moved += implementation_Moved;
            Implementation.MouseEntered += () => cursorInWindow.Value = true;
            Implementation.MouseLeft += () => cursorInWindow.Value = false;

            Bounds.ValueChanged += bounds_ValueChanged;
            InternalSize.ValueChanged += internalSize_ValueChanged;

            Bounds.Value = Implementation.Bounds.ToSystemDrawing();
        }

        private bool boundsChanging;

        private void implementation_Resized()
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            Bounds.Value = Implementation.Bounds.ToSystemDrawing();
            InternalSize.Value = getDrawableSize();
            boundsChanging = false;
        }

        private void implementation_Moved(Veldrid.Point point)
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            Bounds.Value = Implementation.Bounds.ToSystemDrawing();
            boundsChanging = false;
        }

        private void bounds_ValueChanged(ValueChangedEvent<Rectangle> evt)
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            Implementation.X = evt.NewValue.X;
            Implementation.Y = evt.NewValue.Y;
            Implementation.Width = evt.NewValue.Width;
            Implementation.Height = evt.NewValue.Height;
            InternalSize.Value = getDrawableSize();
            boundsChanging = false;
        }

        private void internalSize_ValueChanged(ValueChangedEvent<Size> evt)
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            var borders = getWindowBorders();
            var newBounds = new Rectangle((int)(Implementation.X + borders.Left),
                (int)(Implementation.Y + borders.Top),
                (int)(evt.NewValue.Width + borders.Left + borders.Right),
                (int)(evt.NewValue.Height + borders.Top + borders.Bottom));
            Bounds.Value = newBounds;
            boundsChanging = false;
        }

        public override void Run()
        {
            if (!(Host.Input is VeldridInputBackend input))
                throw new Exception($"{nameof(VeldridWindowBackend)} requires a corresponding {nameof(VeldridInputBackend)}");

            while (Implementation.Exists)
            {
                input.Snapshot = Implementation.PumpEvents();
                OnUpdate();
            }
        }

        public override void Close()
        {
        }

        public override Point PointToClient(Point point) => point;

        public override Point PointToScreen(Point point) => point;

        private unsafe Size getDrawableSize()
        {
            int w, h;
            Sdl2Funcs.SDL_GL_GetDrawableSize(SdlWindow, &w, &h);
            return new Size(w, h);
        }

        private unsafe MarginPadding getWindowBorders()
        {
            int top, left, bottom, right;
            Sdl2Funcs.SDL_GetWindowBordersSize(SdlWindow, &top, &left, &bottom, &right);
            return new MarginPadding { Top = top, Left = left, Bottom = bottom, Right = right };
        }

        #region IDisposable

        private bool isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                }

                isDisposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
