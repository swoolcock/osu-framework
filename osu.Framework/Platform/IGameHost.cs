// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections;
using System.Collections.Generic;
using osu.Framework.Backends;
using osu.Framework.Threading;

namespace osu.Framework.Platform
{
    public interface IGameHost : IBackendProvider, IDisposable
    {
        #region Events

        event Func<bool> ExitRequested;
        event Action Exited;

        /// <summary>
        /// An unhandled exception was thrown. Return true to ignore and continue running.
        /// </summary>
        event Func<Exception, bool> ExceptionThrown;

        #endregion

        #region Execution

        void Run(Game game);

        #endregion

        #region Threading

        GameThread DrawThread { get; }
        GameThread UpdateThread { get; }
        InputThread InputThread { get; }
        AudioThread AudioThread { get; }

        IEnumerable<GameThread> Threads { get; }

        double MaximumUpdateHz { get; set; }
        double MaximumDrawHz { get; set; }
        double MaximumInactiveHz { get; set; }

        /// <summary>
        /// Register a thread to be monitored and tracked by this <see cref="IGameHost"/>
        /// </summary>
        /// <param name="thread">The thread.</param>
        void RegisterThread(GameThread thread);

        /// <summary>
        /// Unregister a thread previously registered with this <see cref="IGameHost"/>
        /// </summary>
        /// <param name="thread">The thread.</param>
        void UnregisterThread(GameThread thread);

        #endregion
    }
}
