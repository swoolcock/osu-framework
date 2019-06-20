// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.IO;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Video;
using osu.Framework.Platform;

namespace osu.Framework.Backends.Video
{
    public class FfmpegVideoBackend : VideoBackend
    {
        public override void Initialise(IGameHost host)
        {
        }

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

        public override VideoDecoder CreateVideoDecoder(Stream stream) => new VideoDecoder(stream);
    }
}
