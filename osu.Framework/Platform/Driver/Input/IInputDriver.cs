// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Platform.Driver.Input
{
    public interface IInputDriver : IDriver
    {
        event Action KeyDown;
        event Action KeyUp;
        event Action KeyPress;
        event Action MouseDown;
        event Action MouseUp;
        event Action MouseMove;
    }
}
