// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;
using osu.Framework.Logging;
using osu.Framework.Timing;

namespace osu.Framework.Platform
{
    /// <summary>
    /// A GameHost which doesn't require a graphical or sound device.
    /// </summary>
    public class HeadlessGameHost : DesktopGameHost
    {
        public const double CLOCK_RATE = 1000.0 / 30;

        private readonly bool realtime;
        private IFrameBasedClock customClock;

        protected override IFrameBasedClock SceneGraphClock => customClock ?? base.SceneGraphClock;

        public override void OpenFileExternally(string filename) => Logger.Log($"Application has requested file \"{filename}\" to be opened.");

        public override void OpenUrlExternally(string url) => Logger.Log($"Application has requested URL \"{url}\" to be opened.");

        public override Storage GetStorage(string baseName) => new DesktopStorage($"headless-{baseName}", this);

        protected override IWindow CreateWindow() => null; // TODO: headless window
        protected override IAudio CreateAudio() => null; // TODO: headless audio
        protected override IGraphics CreateGraphics() => null; // TODO: headless graphics
        protected override IInput CreateInput() => null; // TODO: headless input
        protected override IVideo CreateVideo() => null; // TODO: headless video

        public HeadlessGameHost(string gameName = @"", bool bindIPC = false, bool realtime = true, bool portableInstallation = false)
            : base(gameName, bindIPC, portableInstallation: portableInstallation)
        {
            this.realtime = realtime;
        }

        protected override void SetupForRun()
        {
            base.SetupForRun();

            if (!realtime) customClock = new FramedClock(new FastClock(CLOCK_RATE));
        }

        protected override void UpdateInitialize()
        {
        }

        protected override void DrawInitialize()
        {
        }

        protected override void DrawFrame()
        {
            //we can't draw.
        }

        protected override void UpdateFrame()
        {
            customClock?.ProcessFrame();

            base.UpdateFrame();
        }

        private class FastClock : IClock
        {
            private readonly double increment;
            private double time;

            /// <summary>
            /// A clock which increments each time <see cref="CurrentTime"/> is requested.
            /// Run fast. Run consistent.
            /// </summary>
            /// <param name="increment">Milliseconds we should increment the clock by each time the time is requested.</param>
            public FastClock(double increment)
            {
                this.increment = increment;
            }

            public double CurrentTime => time += increment;
            public double Rate => 1;
            public bool IsRunning => true;
        }
    }
}
