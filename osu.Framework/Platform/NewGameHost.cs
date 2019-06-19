// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Storage;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Statistics;
using osu.Framework.Threading;

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

        public event Action Activated;

        public event Action Deactivated;

        public event Func<bool> ExitRequested;

        public event Action Exited;

        public event Func<Exception, bool> ExceptionThrown;

        #endregion

        #region Event Invocation

        protected virtual void OnActivated() => UpdateThread.Scheduler.Add(() => Activated?.Invoke());

        protected virtual void OnDeactivated() => UpdateThread.Scheduler.Add(() => Deactivated?.Invoke());

        protected virtual bool OnExitRequested() => ExitRequested?.Invoke() ?? false;

        protected virtual void OnExited() => Exited?.Invoke();

        protected virtual bool OnExceptionThrown(Exception exception) => ExceptionThrown?.Invoke(exception) ?? false;

        #endregion

        #region Properties

        public string Name { get; }

        #endregion

        #region Bindables

        public IBindable<bool> IsActive { get; } = new Bindable<bool>(true);

        private readonly Bindable<bool> performanceLogging = new Bindable<bool>();

        #endregion

        #region Backend Creation

        protected abstract IWindow CreateWindow();
        protected abstract IInput CreateInput();
        protected abstract IGraphics CreateGraphics();
        protected abstract IAudio CreateAudio();
        protected abstract IVideo CreateVideo();
        protected abstract IStorage CreateStorage();

        /// <summary>
        /// Creates and initialises the backends for this <see cref="IGameHost"/>, and connects any events and bindables.
        /// </summary>
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

        #endregion

        protected NewGameHost(string gameName = @"")
        {
            Name = gameName;
        }

        #region Execution

        public void Run(Game game)
        {
            CreateBackends();

            RegisterThread(DrawThread = new DrawThread(DrawFrame)
            {
                OnThreadStart = DrawInitialize,
            });

            RegisterThread(UpdateThread = new UpdateThread(UpdateFrame)
            {
                OnThreadStart = UpdateInitialize,
                Monitor = { HandleGC = true },
            });

            RegisterThread(InputThread = new InputThread());
            RegisterThread(AudioThread = new AudioThread());
        }

        protected virtual void UpdateInitialize()
        {
        }

        protected virtual void UpdateFrame()
        {
        }

        protected virtual void DrawInitialize()
        {
        }

        protected virtual void DrawFrame()
        {
        }

        #endregion

        #region Exception Handling

        private void unhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = (Exception)args.ExceptionObject;
            exception.Data["unhandled"] = "unhandled";
            handleException(exception);
        }

        private void unobservedExceptionHandler(object sender, UnobservedTaskExceptionEventArgs args)
        {
            args.Exception.Data["unhandled"] = "unobserved";
            handleException(args.Exception);
        }

        private void handleException(Exception exception)
        {
            if (OnExceptionThrown(exception))
            {
                AppDomain.CurrentDomain.UnhandledException -= unhandledExceptionHandler;

                var captured = ExceptionDispatchInfo.Capture(exception);

                //we want to throw this exception on the input thread to interrupt window and also headless execution.
                InputThread.Scheduler.Add(() => { captured.Throw(); });
            }

            Logger.Error(exception, $"An {exception.Data["unhandled"]} error has occurred.", recursive: true);
        }

        #endregion

        #region Threading

        public GameThread DrawThread { get; private set; }
        public GameThread UpdateThread { get; private set; }
        public InputThread InputThread { get; private set; }
        public AudioThread AudioThread { get; private set; }

        private double maximumUpdateHz;

        public double MaximumUpdateHz
        {
            get => maximumUpdateHz;
            set => UpdateThread.ActiveHz = maximumUpdateHz = value;
        }

        private double maximumDrawHz;

        public double MaximumDrawHz
        {
            get => maximumDrawHz;
            set => DrawThread.ActiveHz = maximumDrawHz = value;
        }

        public double MaximumInactiveHz
        {
            get => DrawThread.InactiveHz;
            set
            {
                DrawThread.InactiveHz = value;
                UpdateThread.InactiveHz = value;
            }
        }

        private readonly List<GameThread> threads = new List<GameThread>();

        public IEnumerable<GameThread> Threads => threads;

        public void RegisterThread(GameThread thread)
        {
            threads.Add(thread);
            thread.IsActive.BindTo(IsActive);
            thread.UnhandledException = unhandledExceptionHandler;
            thread.Monitor.EnablePerformanceProfiling = performanceLogging.Value;
        }

        public void UnregisterThread(GameThread thread)
        {
            if (!threads.Remove(thread))
                return;

            IsActive.UnbindFrom(thread.IsActive);
            thread.UnhandledException = null;
        }

        #endregion

        #region Performance Monitoring

        private PerformanceMonitor inputMonitor => InputThread.Monitor;
        private PerformanceMonitor drawMonitor => DrawThread.Monitor;

        #endregion

        #region IDisposable

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    inputMonitor?.Dispose();
                    drawMonitor?.Dispose();

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
