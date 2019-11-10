// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Drawing;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Platform;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace osu.Framework.Backends.Window
{
    public class VeldridWindowBackend : WindowBackend
    {
        internal readonly Sdl2Window Implementation;

        #region Read-only Bindables

        private readonly BindableBool focused = new BindableBool();
        public override IBindable<bool> Focused => focused;

        private readonly BindableBool cursorInWindow = new BindableBool();
        public override IBindable<bool> CursorInWindow => cursorInWindow;

        private readonly BindableBool visible = new BindableBool();
        public override IBindable<bool> Visible => visible;

        #endregion

        public override void Initialise(IGameHost host)
        {
        }

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

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
        }

        public override void Run()
        {
            while (Implementation.Exists)
            {
                Implementation.PumpEvents();
            }
        }

        public override void Close()
        {
        }

        public override Point PointToClient(Point point) => point;

        public override Point PointToScreen(Point point) => point;

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
