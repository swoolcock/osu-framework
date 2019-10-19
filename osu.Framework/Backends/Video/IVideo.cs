// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using osu.Framework.Graphics.Video;
using osu.Framework.Threading;

namespace osu.Framework.Backends.Video
{
    /// <summary>
    /// Provides video decoders.
    /// Assumption here is that <see cref="VideoDecoder"/> will be an abstract class, realised by "FfmpegVideoDecoder" or similar.
    /// </summary>
    public interface IVideo : IBackend
    {
        VideoDecoder CreateVideoDecoder(Stream stream, Scheduler scheduler);
    }
}
