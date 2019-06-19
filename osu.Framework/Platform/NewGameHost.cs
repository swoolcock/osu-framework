// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Storage;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;

namespace osu.Framework.Platform
{
    public abstract class NewGameHost : IGameHost
    {
        #region IBackendProvider

        public IWindow Window { get; private set; }
        public IInput Input { get; private set; }
        public IGraphics Graphics { get; private set; }
        public IAudio Audio { get; private set; }
        public IVideo Video { get; private set; }
        public IStorage Storage { get; private set; }

        #endregion

        #region Events

        public event Func<bool> ExitRequested;

        public event Action Exited;

        #endregion

        #region Event Invocation

        protected virtual bool OnExitRequested() => ExitRequested?.Invoke() ?? false;

        protected virtual void OnExited() => Exited?.Invoke();

        #endregion

        protected abstract IWindow CreateWindow();
        protected abstract IInput CreateInput();
        protected abstract IGraphics CreateGraphics();
        protected abstract IAudio CreateAudio();
        protected abstract IVideo CreateVideo();
        protected abstract IStorage CreateStorage();

        protected NewGameHost()
        {
            CreateBackends();
        }

        protected virtual void CreateBackends()
        {
            // create backends from virtual methods
            Window = CreateWindow();
            Input = CreateInput();
            Graphics = CreateGraphics();
            Audio = CreateAudio();
            Video = CreateVideo();
            Storage = CreateStorage();

            // initialise backends
            Window.Initialise(this);
            Input.Initialise(this);
            Graphics.Initialise(this);
            Audio.Initialise(this);
            Video.Initialise(this);
            Storage.Initialise(this);

            // connect backend events to gamehost
            Window.CloseRequested += OnExitRequested;
            Window.Closed += OnExited;
        }

        public void Run(Game game)
        {
        }

        #region IDisposable

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Storage?.Dispose();
                    Video?.Dispose();
                    Audio?.Dispose();
                    Graphics?.Dispose();
                    Input?.Dispose();
                    Window?.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NewGameHost()
        {
            Dispose(false);
        }

        #endregion
    }
}
