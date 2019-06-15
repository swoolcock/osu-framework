// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Platform.Driver.Input;
using osuTK.Graphics;
using osuTK.Platform;

namespace osu.Framework.Platform.Driver.Window
{
    public class OsuTKWindowDriver : WindowDriver
    {
        internal readonly IGameWindow Implementation;

        public OsuTKWindowDriver(IGameWindow implementation)
        {
            Implementation = implementation;
        }

        public OsuTKWindowDriver(int width, int height)
            : this(new osuTK.GameWindow(width, height, new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, GraphicsMode.Default.Samples, GraphicsMode.Default.AccumulatorFormat, 3)))
        {
        }

        public override void Initialise(IDriverProvider provider)
        {
            Implementation.Closing += (sender, e) => e.Cancel = OnCloseRequested();
            Implementation.Closed += (sender, e) => OnClosed();
        }
    }
}
