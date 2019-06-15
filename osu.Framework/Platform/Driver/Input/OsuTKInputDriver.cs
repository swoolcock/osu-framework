// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Platform.Driver.Window;

namespace osu.Framework.Platform.Driver.Input
{
    public class OsuTKInputDriver : InputDriver
    {
        public override void Initialise(IDriverProvider provider)
        {
            if (!(provider.Window is OsuTKWindowDriver window))
                throw new Exception($"{typeof(OsuTKInputDriver)} requires a matching {typeof(OsuTKWindowDriver)}");

            window.Implementation.KeyDown += (sender, e) => OnKeyDown();
            window.Implementation.KeyUp += (sender, e) => OnKeyUp();
            window.Implementation.KeyPress += (sender, e) => OnKeyPress();
            window.Implementation.MouseDown += (sender, e) => OnMouseDown();
            window.Implementation.MouseUp += (sender, e) => OnMouseUp();
            window.Implementation.MouseMove += (sender, e) => OnMouseMove();
        }
    }
}
