// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;

namespace osu.Framework.Platform
{
    public class NewDesktopGameHost : NewGameHost
    {
        private const int default_window_width = 1366;
        private const int default_window_height = 768;

        protected override IWindowDriver CreateWindow() => new OsuTKWindowDriver(default_window_width, default_window_height);

        protected override IInputDriver CreateInput() => new OsuTKInputDriver();

        protected override IGraphicsDriver CreateGraphics() => new OsuTKGraphicsDriver();

        protected override IAudioDriver CreateAudio() => new BassAudioDriver();

        protected override IVideoDriver CreateVideo() => new FfmpegVideoDriver();
    }
}
