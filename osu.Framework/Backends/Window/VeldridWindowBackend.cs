// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Drawing;
using osu.Framework.Backends.Input;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Configuration;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Platform;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

// ReSharper disable InconsistentNaming

namespace osu.Framework.Backends.Window
{
    public class VeldridWindowBackend : WindowBackend
    {
        internal readonly Sdl2Window Implementation;
        internal IntPtr SdlWindow;

        private readonly Cached<float> scale = new Cached<float>();

        internal float Scale
        {
            get
            {
                if (scale.IsValid)
                    return scale.Value;

                var borders = getWindowBorders();
                var bounds = Implementation.Bounds;
                float realWidth = bounds.Width - borders.Left - borders.Right;
                float scaledWidth = getDrawableSize().Width;
                scale.Value = scaledWidth / realWidth;
                return scale.Value;
            }
        }

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
                WindowTitle = "Test",
                WindowInitialState = Veldrid.WindowState.Normal
            };
            // Implementation = VeldridStartup.CreateWindow(ref windowCi);
            SDL_WindowFlags flags = SDL_WindowFlags.OpenGL |
                                    SDL_WindowFlags.Resizable |
                                    SDL_WindowFlags.AllowHighDpi |
                                    GetWindowFlags(windowCi.WindowInitialState);
            Implementation = new Sdl2Window(windowCi.WindowTitle, windowCi.X, windowCi.Y, windowCi.WindowWidth, windowCi.WindowHeight, flags, false);
            SdlWindow = Implementation.SdlWindowHandle;

            Implementation.FocusGained += () => focused.Value = true;
            Implementation.FocusLost += () => focused.Value = false;
            Implementation.Resized += implementation_Resized;
            Implementation.Moved += implementation_Moved;
            Implementation.MouseEntered += () => cursorInWindow.Value = true;
            Implementation.MouseLeft += () => cursorInWindow.Value = false;
            Implementation.Hidden += implementation_HiddenShown;
            Implementation.Shown += implementation_HiddenShown;
            Implementation.Closed += OnClosed;

            Bounds.ValueChanged += bounds_ValueChanged;
            InternalSize.ValueChanged += internalSize_ValueChanged;
            CursorState.ValueChanged += cursorState_ValueChanged;

            Bounds.Value = Implementation.Bounds.ToSystemDrawing();
            visible.Value = Implementation.Visible;
            CursorState.Value = Platform.CursorState.Default;
        }

        private void cursorState_ValueChanged(ValueChangedEvent<CursorState> evt)
        {
            switch (evt.NewValue)
            {
                case Platform.CursorState.Default:
                    Implementation.CursorVisible = true;
                    break;

                case Platform.CursorState.Hidden:
                    Implementation.CursorVisible = false;
                    break;

                case Platform.CursorState.Confined:
                    Implementation.CursorVisible = true;
                    break;

                case Platform.CursorState.HiddenAndConfined:
                    Implementation.CursorVisible = false;
                    break;
            }
        }

        private bool visibleChanging;

        private void implementation_HiddenShown()
        {
            if (visibleChanging)
                return;

            visibleChanging = true;
            visible.Value = Implementation.Visible;
            visibleChanging = false;
        }

        private bool boundsChanging;

        private void implementation_Resized()
        {
            if (boundsChanging)
                return;

            boundsChanging = true;
            Bounds.Value = Implementation.Bounds.ToSystemDrawing();
            InternalSize.Value = getDrawableSize();
            scale.Invalidate();
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
            scale.Invalidate();
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
            scale.Invalidate();
            boundsChanging = false;
        }

        public override void Run()
        {
            if (!(Host.Input is VeldridInputBackend input))
                throw new Exception($"{nameof(VeldridWindowBackend)} requires a corresponding {nameof(VeldridInputBackend)}");

            while (Implementation.Exists)
            {
                input.Snapshot = Implementation.PumpEvents();
                input.TriggerKeypresses();
                OnUpdate();
            }
        }

        public override void Close()
        {
            if (!OnCloseRequested())
                Implementation.Close();
        }

        public override Point PointToClient(Point point) => Implementation.ScreenToClient(point.ToVeldrid()).ToSystemDrawing();

        public override Point PointToScreen(Point point) => Implementation.ClientToScreen(point.ToVeldrid()).ToSystemDrawing();

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

        private static SDL_WindowFlags GetWindowFlags(WindowState state)
        {
            switch (state)
            {
                case Veldrid.WindowState.Normal:
                    return 0;

                case Veldrid.WindowState.FullScreen:
                    return SDL_WindowFlags.Fullscreen;

                case Veldrid.WindowState.Maximized:
                    return SDL_WindowFlags.Maximized;

                case Veldrid.WindowState.Minimized:
                    return SDL_WindowFlags.Minimized;

                case Veldrid.WindowState.BorderlessFullScreen:
                    return SDL_WindowFlags.FullScreenDesktop;

                case Veldrid.WindowState.Hidden:
                    return SDL_WindowFlags.Hidden;
            }

            return 0;
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
