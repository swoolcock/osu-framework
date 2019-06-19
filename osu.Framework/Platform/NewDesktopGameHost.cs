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

        protected override IWindowBackend CreateWindow() => new OsuTKWindowBackend(default_window_width, default_window_height);

        protected override IInputBackend CreateInput() => new OsuTKInputBackend();

        protected override IGraphicsBackend CreateGraphics() => new OsuTKGraphicsBackend();

        protected override IAudioBackend CreateAudio() => new BassAudioBackend();

        protected override IVideoBackend CreateVideo() => new FfmpegVideoBackend();

        protected override IStorageBackend CreateStorage() => throw new NotImplementedException();
    }
}
