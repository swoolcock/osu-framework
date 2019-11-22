// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Numerics;
using osu.Framework.Caching;
using osu.Framework.Threading;
using Veldrid;
using Veldrid.Sdl2;

namespace osu.Framework.Platform
{
    /// <summary>
    /// Implementation of <see cref="IWindowBackend"/> that uses an SDL2 window.
    /// </summary>
    public class Sdl2WindowBackend : IWindowBackend
    {
        private const int default_width = 1366;
        private const int default_height = 768;

        private Sdl2Window implementation;
        private InputSnapshot inputSnapshot;
        private readonly Scheduler scheduler = new Scheduler();

        #region Internal Properties

        internal IntPtr SdlWindowHandle => implementation?.SdlWindowHandle ?? IntPtr.Zero;

        #endregion

        #region IWindowBackend.Properties

        private string title = "";

        public string Title
        {
            get => title;
            set
            {
                title = value;

                scheduler.Add(() => implementation.Title = $"{value} (SDL)");
            }
        }

        private bool visible;

        public bool Visible
        {
            get => implementation?.Visible ?? visible;
            set
            {
                visible = value;

                scheduler.Add(() => implementation.Visible = value);
            }
        }

        public bool Exists => implementation?.Exists ?? false;

        private Vector2 position = Vector2.Zero;

        public Vector2 Position
        {
            get => implementation == null ? position : new Vector2(implementation.X, implementation.Y);
            set
            {
                position = value;

                scheduler.Add(() =>
                {
                    implementation.X = (int)value.X;
                    implementation.Y = (int)value.Y;
                });
            }
        }

        private Vector2 size = new Vector2(default_width, default_height);

        public Vector2 Size
        {
            get => implementation == null ? size : new Vector2(implementation.Width, implementation.Height);
            set
            {
                size = value;

                scheduler.Add(() =>
                {
                    implementation.Width = (int)value.X;
                    implementation.Height = (int)value.Y;
                });
            }
        }

        private readonly Cached<float> scale = new Cached<float>();

        public float Scale
        {
            get
            {
                if (scale.IsValid)
                    return scale.Value;

                float realWidth = implementation.Width;
                float scaledWidth = Sdl2Functions.SDL_GL_GetDrawableSize(SdlWindowHandle).X;
                scale.Value = scaledWidth / realWidth;
                return scale.Value;
            }
        }

        private bool cursorVisible = true;

        public bool CursorVisible
        {
            get => implementation?.CursorVisible ?? cursorVisible;
            set
            {
                cursorVisible = value;

                scheduler.Add(() => implementation.CursorVisible = value);
            }
        }

        public bool CursorConfined { get; set; }

        private WindowState windowState;

        public WindowState WindowState
        {
            get => implementation?.WindowState ?? windowState;
            set
            {
                windowState = value;

                scheduler.Add(() => implementation.WindowState = value);
            }
        }

        #endregion

        private readonly Cached<Display[]> displays = new Cached<Display[]>();

        public IEnumerable<Display> Displays
        {
            get
            {
                if (displays.IsValid)
                    return displays.Value;

                int numVideoDisplays = Sdl2Functions.SDL_GetNumVideoDisplays();
                var displayList = new List<Display>();

                for (int displayIndex = 0; displayIndex < numVideoDisplays; displayIndex++)
                {
                    int numModes = Sdl2Functions.SDL_GetNumDisplayModes(displayIndex);
                    var modes = new List<DisplayMode>();

                    for (int modeIndex = 0; modeIndex < numModes; modeIndex++)
                    {
                        DisplayMode? mode = Sdl2Functions.SDL_GetDisplayMode(displayIndex, modeIndex);
                        if (mode.HasValue)
                            modes.Add(mode.Value);
                    }

                    displayList.Add(new Display(displayIndex, modes.ToArray()));
                }

                displays.Value = displayList.ToArray();

                return displays.Value;
            }
        }

        #region IWindowBackend.Events

        public event Action Update;
        public event Action Resized;
        public event Action WindowStateChanged;
        public event Func<bool> CloseRequested;
        public event Action Closed;
        public event Action FocusLost;
        public event Action FocusGained;
        public event Action Shown;
        public event Action Hidden;
        public event Action MouseEntered;
        public event Action MouseLeft;
        public event Action<Point> Moved;
        public event Action<MouseWheelEventArgs> MouseWheel;
        public event Action<MouseMoveEventArgs> MouseMove;
        public event Action<MouseEvent> MouseDown;
        public event Action<MouseEvent> MouseUp;
        public event Action<KeyEvent> KeyDown;
        public event Action<KeyEvent> KeyUp;
        public event Action<char> KeyTyped;
        public event Action<DragDropEvent> DragDrop;

        #endregion

        #region Event Invocation

        protected virtual void OnUpdate() => Update?.Invoke();
        protected virtual void OnResized() => Resized?.Invoke();
        protected virtual void OnWindowStateChanged() => WindowStateChanged?.Invoke();
        protected virtual bool OnCloseRequested() => CloseRequested?.Invoke() ?? false;
        protected virtual void OnClosed() => Closed?.Invoke();
        protected virtual void OnFocusLost() => FocusLost?.Invoke();
        protected virtual void OnFocusGained() => FocusGained?.Invoke();
        protected virtual void OnShown() => Shown?.Invoke();
        protected virtual void OnHidden() => Hidden?.Invoke();
        protected virtual void OnMouseEntered() => MouseEntered?.Invoke();
        protected virtual void OnMouseLeft() => MouseLeft?.Invoke();
        protected virtual void OnMoved(Point point) => Moved?.Invoke(point);
        protected virtual void OnMouseWheel(MouseWheelEventArgs args) => MouseWheel?.Invoke(args);
        protected virtual void OnMouseMove(MouseMoveEventArgs args) => MouseMove?.Invoke(args);
        protected virtual void OnMouseDown(MouseEvent evt) => MouseDown?.Invoke(evt);
        protected virtual void OnMouseUp(MouseEvent evt) => MouseUp?.Invoke(evt);
        protected virtual void OnKeyDown(KeyEvent evt) => KeyDown?.Invoke(evt);
        protected virtual void OnKeyUp(KeyEvent evt) => KeyUp?.Invoke(evt);
        protected virtual void OnKeyTyped(char c) => KeyTyped?.Invoke(c);
        protected virtual void OnDragDrop(DragDropEvent evt) => DragDrop?.Invoke(evt);

        #endregion

        #region IWindowBackend.Methods

        public void Create()
        {
            SDL_WindowFlags flags = SDL_WindowFlags.OpenGL |
                                    SDL_WindowFlags.Resizable |
                                    SDL_WindowFlags.AllowHighDpi |
                                    getWindowFlags(WindowState);

            implementation = new Sdl2Window(Title, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, flags, false);

            // force a refresh of the size and position now that we can calculate the scale
            scale.Invalidate();
            Size = size;
            Position = position;

            implementation.MouseDown += OnMouseDown;
            implementation.MouseUp += OnMouseUp;
            implementation.MouseMove += OnMouseMove;
            implementation.MouseWheel += OnMouseWheel;
            implementation.KeyDown += OnKeyDown;
            implementation.KeyUp += OnKeyUp;
            implementation.FocusGained += OnFocusGained;
            implementation.FocusLost += OnFocusLost;
            implementation.Resized += implementation_Resized;
            implementation.Moved += OnMoved;
            implementation.MouseEntered += OnMouseEntered;
            implementation.MouseLeft += OnMouseLeft;
            implementation.Hidden += OnHidden;
            implementation.Shown += OnShown;
            implementation.Closed += OnClosed;
            implementation.DragDrop += OnDragDrop;
        }

        public void Run()
        {
            while (implementation.Exists)
            {
                inputSnapshot = implementation.PumpEvents();

                foreach (var c in inputSnapshot.KeyCharPresses)
                    OnKeyTyped(c);

                OnUpdate();

                scheduler.Update();
            }
        }

        public void Close()
        {
            // TODO: Fix OnCloseRequested
            // The Sdl2Window implementation does not currently have a way of aborting a manual close request.
            // The best we can do for now is abort any programmatic close requests if required.
            if (!OnCloseRequested())
                scheduler.Add(implementation.Close);
        }

        #endregion

        private void implementation_Resized()
        {
            if (implementation.WindowState != windowState)
                OnWindowStateChanged();

            OnResized();
        }

        private static SDL_WindowFlags getWindowFlags(WindowState state)
        {
            switch (state)
            {
                case WindowState.Normal:
                    return 0;

                case WindowState.FullScreen:
                    return SDL_WindowFlags.Fullscreen;

                case WindowState.Maximized:
                    return SDL_WindowFlags.Maximized;

                case WindowState.Minimized:
                    return SDL_WindowFlags.Minimized;

                case WindowState.BorderlessFullScreen:
                    return SDL_WindowFlags.FullScreenDesktop;

                case WindowState.Hidden:
                    return SDL_WindowFlags.Hidden;
            }

            return 0;
        }
    }
}
