// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using osu.Framework.Graphics.Video;

namespace osu.Framework.Backends.Video
{
    public class FfmpegVideoBackend : VideoBackend
    {
        public override void Initialise(IBackendProvider provider)
        {
        }

        public override VideoDecoder CreateVideoDecoder(Stream stream) => new VideoDecoder(stream);
    }
}
