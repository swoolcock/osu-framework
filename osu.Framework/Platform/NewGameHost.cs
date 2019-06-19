// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;

namespace osu.Framework.Platform
{
    public abstract class NewGameHost : IGameHost
    {
        #region IDriverProvider

        public IWindowDriver Window { get; private set; }
        public IInputDriver Input { get; private set; }
        public IGraphicsDriver Graphics { get; private set; }
        public IAudioDriver Audio { get; private set; }
        public IVideoDriver Video { get; private set; }

        #endregion

        #region Events

        public event Func<bool> ExitRequested;

        public event Action Exited;

        #endregion

        #region Event Invocation

        protected virtual bool OnExitRequested() => ExitRequested?.Invoke() ?? false;

        protected virtual void OnExited() => Exited?.Invoke();

        #endregion

        protected abstract IWindowDriver CreateWindow();
        protected abstract IInputDriver CreateInput();
        protected abstract IGraphicsDriver CreateGraphics();
        protected abstract IAudioDriver CreateAudio();
        protected abstract IVideoDriver CreateVideo();

        protected NewGameHost()
        {
            CreateDrivers();
        }

        protected virtual void CreateDrivers()
        {
            // create drivers from virtual methods
            Window = CreateWindow();
            Input = CreateInput();
            Graphics = CreateGraphics();
            Audio = CreateAudio();
            Video = CreateVideo();

            // initialise drivers
            Window.Initialise(this);
            Input.Initialise(this);
            Graphics.Initialise(this);
            Audio.Initialise(this);
            Video.Initialise(this);

            // connect driver events to gamehost
            Window.CloseRequested += OnExitRequested;
            Window.Closed += OnExited;
        }

        #region IDisposable

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
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
