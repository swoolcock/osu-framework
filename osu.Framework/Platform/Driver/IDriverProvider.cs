// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Platform.Driver.Audio;
using osu.Framework.Platform.Driver.Graphics;
using osu.Framework.Platform.Driver.Input;
using osu.Framework.Platform.Driver.Video;
using osu.Framework.Platform.Driver.Window;

namespace osu.Framework.Platform.Driver
{
    /// <summary>
    /// Provides concrete implementations of various <see cref="IDriver"/> classes required by the game.
    /// </summary>
    public interface IDriverProvider
    {
        /// <summary>
        /// Provides the concrete implementation of the window.
        /// </summary>
        IWindowDriver Window { get; }

        /// <summary>
        /// Provides the concrete implementation of the input driver.
        /// </summary>
        IInputDriver Input { get; }

        /// <summary>
        /// Provides the concrete implementation of the graphics driver.
        /// </summary>
        IGraphicsDriver Graphics { get; }

        /// <summary>
        /// Provides the concrete implementation of the audio driver.
        /// </summary>
        IAudioDriver Audio { get; }

        /// <summary>
        /// Provides the concrete implementation of the video driver.
        /// </summary>
        IVideoDriver Video { get; }
    }
}
