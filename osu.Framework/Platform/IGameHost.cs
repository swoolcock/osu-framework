// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends;

namespace osu.Framework.Platform
{
    public interface IGameHost : IBackendProvider, IDisposable
    {
        #region Events

        event Func<bool> ExitRequested;
        event Action Exited;

        #endregion

        void Run(Game game);
    }
}
