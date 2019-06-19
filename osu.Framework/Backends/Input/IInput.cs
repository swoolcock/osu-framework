// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Input.Handlers;

namespace osu.Framework.Backends.Input
{
    /// <summary>
    /// Provides input events and instantiates <see cref="InputHandler"/>s for those events.
    /// </summary>
    public interface IInput : IBackend
    {
        #region Events

        event Action KeyDown;
        event Action KeyUp;
        event Action KeyPress;
        event Action MouseDown;
        event Action MouseUp;
        event Action MouseMove;

        #endregion

        IEnumerable<InputHandler> CreateInputHandlers();
    }
}
