// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Platform.Driver.Audio;
using osu.Framework.Platform.Driver.Graphics;
using osu.Framework.Platform.Driver.Input;
using osu.Framework.Platform.Driver.Video;
using osu.Framework.Platform.Driver.Window;

namespace osu.Framework.Platform.Driver
{
    public interface IDriverProvider
    {
        IWindowDriver Window { get; }
        IInputDriver Input { get; }
        IGraphicsDriver Graphics { get; }
        IAudioDriver Audio { get; }
        IVideoDriver Video { get; }
    }
}
