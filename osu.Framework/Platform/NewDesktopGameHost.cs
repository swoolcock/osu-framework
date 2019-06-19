// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Storage;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;

namespace osu.Framework.Platform
{
    public class NewDesktopGameHost : NewGameHost
    {
        private const int default_window_width = 1366;
        private const int default_window_height = 768;

        protected override IWindow CreateWindow() => new OsuTKWindowBackend(default_window_width, default_window_height);

        protected override IInput CreateInput() => new OsuTKInputBackend();

        protected override IGraphics CreateGraphics() => new OsuTKGraphicsBackend();

        protected override IAudio CreateAudio() => new BassAudioBackend();

        protected override IVideo CreateVideo() => new FfmpegVideoBackend();

        protected override IStorage CreateStorage() => throw new NotImplementedException();
    }
}
