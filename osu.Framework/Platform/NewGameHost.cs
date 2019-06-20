// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Storage;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Development;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Input;
using osu.Framework.IO.File;
using osu.Framework.Logging;
using osu.Framework.Statistics;
using osu.Framework.Threading;
using osu.Framework.Timing;
using osuTK;
using osuTK.Graphics.ES30;

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
            Window = CreateWindow();
            Input = CreateInput();
            Graphics = CreateGraphics();
            Audio = CreateAudio();
            Video = CreateVideo();
            // Storage = CreateStorage();
        }

        protected virtual void InitialiseBackends()
        {
            Window.Initialise(this);
            Input.Initialise(this);
            Graphics.Initialise(this);
            Audio.Initialise(this);
            Video.Initialise(this);
            // Storage.Initialise(this);
        }

        protected virtual void ConfigureBackends(ConfigManager<FrameworkSetting> config)
        {
            Window.Configure(config);
            Input.Configure(config);
            Graphics.Configure(config);
            Audio.Configure(config);
            Video.Configure(config);
            // Storage.Configure(config);
        }

        private void connectBackends()
        {
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

        public ExecutionState ExecutionState { get; private set; }

        public DependencyContainer Dependencies { get; } = new DependencyContainer();

        protected TripleBuffer<DrawNode> DrawRoots = new TripleBuffer<DrawNode>();

        /// <summary>
        /// The clock which is to be used by the scene graph (will be assigned to <see cref="Root"/>).
        /// </summary>
        protected virtual IFrameBasedClock SceneGraphClock => UpdateThread.Clock;

        protected Container Root;

        private ulong frameCount;
        private long lastDrawFrameId;

        public void Run(Game game)
        {
            DebugUtils.HostAssembly = game.GetType().Assembly;

            if (ExecutionState != ExecutionState.Idle)
                throw new InvalidOperationException("A game that has already been run cannot be restarted.");

            try
            {
                AppDomain.CurrentDomain.UnhandledException += unhandledExceptionHandler;
                TaskScheduler.UnobservedTaskException += unobservedExceptionHandler;

                CreateBackends();
                InitialiseBackends();

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

                Trace.Listeners.Clear();
                Trace.Listeners.Add(new ThrowingTraceListener());

                FileSafety.DeleteCleanupDirectory();

                var assembly = DebugUtils.GetEntryAssembly();
                string assemblyPath = DebugUtils.GetEntryPath();

                Logger.GameIdentifier = Name;
                Logger.VersionIdentifier = assembly.GetName().Version.ToString();

                if (assemblyPath != null)
                    Environment.CurrentDirectory = assemblyPath;

                Dependencies.CacheAs<IGameHost>(this);
                Dependencies.CacheAs(Window);
                Dependencies.CacheAs(Input);
                Dependencies.CacheAs(Graphics);
                Dependencies.CacheAs(Audio);
                Dependencies.CacheAs(Video);
                // Dependencies.CacheAs(Storage);

                // TODO: SetupForRun();

                ExecutionState = ExecutionState.Running;

                SetupConfig(game.GetFrameworkConfigDefaults());
                ConfigureBackends(Config);

                connectBackends();

                Window.Title.Value = $@"osu!framework (running ""{Name}"")";
                // IsActive.BindTo(Window.IsActive);

                // TODO: resetInputHandlers();

                foreach (var t in threads)
                    t.Start();

                DrawThread.WaitUntilInitialized();
                bootstrapSceneGraph(game);

                try
                {
                    if (Window is OsuTKWindowBackend window)
                    {
                        window.Implementation.UpdateFrame += delegate
                        {
                            inputPerformanceCollectionPeriod?.Dispose();
                            InputThread.RunUpdate();
                            inputPerformanceCollectionPeriod = inputMonitor.BeginCollecting(PerformanceCollectionType.WndProc);
                        };

                        window.Implementation.Closed += delegate
                        {
                            //we need to ensure all threads have stopped before the window is closed (mainly the draw thread
                            //to avoid GL operations running post-cleanup).
                            stopAllThreads();
                        };

                        window.Implementation.Run();
                    }
                }
                catch (OutOfMemoryException)
                {
                }
            }
            finally
            {
                // Close the window and stop all threads
                PerformExit(true);
            }
        }

        /// <summary>
        /// Schedules the game to exit in the next frame.
        /// </summary>
        public void Exit() => PerformExit(false);

        /// <summary>
        /// Schedules the game to exit in the next frame (or immediately if <paramref name="immediately"/> is true).
        /// </summary>
        /// <param name="immediately">If true, exits the game immediately.  If false (default), schedules the game to exit in the next frame.</param>
        protected virtual void PerformExit(bool immediately)
        {
            if (immediately)
                exit();
            else
            {
                ExecutionState = ExecutionState.Stopping;
                InputThread.Scheduler.Add(exit, false);
            }
        }

        /// <summary>
        /// Exits the game. This must always be called from <see cref="InputThread"/>.
        /// </summary>
        private void exit()
        {
            // exit() may be called without having been scheduled from Exit(), so ensure the correct exiting state
            ExecutionState = ExecutionState.Stopping;
            // TODO: Window?.Close();
            stopAllThreads();
            ExecutionState = ExecutionState.Stopped;
        }

        private void setVSyncMode()
        {
            if (Window == null) return;

            // TODO: DrawThread.Scheduler.Add(() => Window.VSync = frameSyncMode.Value == FrameSync.VSync ? VSyncMode.On : VSyncMode.Off);
        }

        protected virtual void UpdateInitialize()
        {
            // TODO: probably don't need this any more?
            //this was added due to the dependency on GLWrapper.MaxTextureSize begin initialised.
            DrawThread.WaitUntilInitialized();
        }

        protected virtual void UpdateFrame()
        {
            if (Root == null) return;

            frameCount++;

            // if (Window == null)
            // {
            //     var windowedSize = Config.Get<Size>(FrameworkSetting.WindowedSize);
            //     Root.Size = new Vector2(windowedSize.Width, windowedSize.Height);
            // }
            // else if (Window.WindowState != WindowState.Minimized)
            //     Root.Size = new Vector2(Window.ClientSize.Width, Window.ClientSize.Height);
            var internalSize = Window.InternalSize.Value;
            Root.Size = new Vector2(internalSize.Width, internalSize.Height);

            // Ensure we maintain a valid size for any children immediately scaling by the window size
            Root.Size = Vector2.ComponentMax(Vector2.One, Root.Size);

            try
            {
                Root.UpdateSubTree();
            }
            catch (DependencyInjectionException die)
            {
                die.DispatchInfo.Throw();
            }

            Root.UpdateSubTreeMasking(Root, Root.ScreenSpaceDrawQuad.AABBFloat);

            using (var buffer = DrawRoots.Get(UsageType.Write))
                buffer.Object = Root.GenerateDrawNodeSubtree(frameCount, buffer.Index, false);
        }

        protected virtual void DrawInitialize()
        {
            // TODO: use backends

            if (!(Window is OsuTKWindowBackend window))
                return;

            window.Implementation.MakeCurrent();
            // TODO: GLWrapper.Initialize(this);

            setVSyncMode();

            GLWrapper.Reset(new Vector2(Window.InternalSize.Value.Width, Window.InternalSize.Value.Height));
        }

        protected virtual void DrawFrame()
        {
            if (Root == null)
                return;

            // TODO: use backends
            while (ExecutionState > ExecutionState.Stopping)
            {
                using (var buffer = DrawRoots.Get(UsageType.Read))
                {
                    if (buffer?.Object == null || buffer.FrameId == lastDrawFrameId)
                    {
                        using (drawMonitor.BeginCollecting(PerformanceCollectionType.Sleep))
                            Thread.Sleep(1);
                        continue;
                    }

                    using (drawMonitor.BeginCollecting(PerformanceCollectionType.GLReset))
                        GLWrapper.Reset(new Vector2(Window.InternalSize.Value.Width, Window.InternalSize.Value.Height));

                    if (!bypassFrontToBackPass.Value)
                    {
                        var depthValue = new DepthValue();

                        GLWrapper.PushDepthInfo(DepthInfo.Default);

                        // Front pass
                        buffer.Object.DrawOpaqueInteriorSubTree(depthValue, null);

                        GLWrapper.PopDepthInfo();

                        // The back pass doesn't write depth, but needs to depth test properly
                        GLWrapper.PushDepthInfo(new DepthInfo(true, false));
                    }
                    else
                    {
                        // Disable depth testing
                        GLWrapper.PushDepthInfo(new DepthInfo());
                    }

                    // Back pass
                    buffer.Object.Draw(null);

                    GLWrapper.PopDepthInfo();

                    lastDrawFrameId = buffer.FrameId;
                    break;
                }
            }

            GLWrapper.FlushCurrentBatch();

            using (drawMonitor.BeginCollecting(PerformanceCollectionType.SwapBuffer))
            {
                // TODO: use backends
                if (Window is OsuTKWindowBackend window)
                {
                    window.Implementation.SwapBuffers();

                    // if (window.Implementation.VSync == VSyncMode.On)
                    //     // without glFinish, vsync is basically unplayable due to the extra latency introduced.
                    //     // we will likely want to give the user control over this in the future as an advanced setting.
                    GL.Finish();
                }
            }
        }

        private void bootstrapSceneGraph(Game game)
        {
            var root = game.CreateUserInputManager();
            root.Child = new PlatformActionContainer
            {
                Child = new FrameworkActionContainer
                {
                    Child = game
                }
            };

            Dependencies.Cache(root);
            Dependencies.CacheAs(game);

            // TODO: game.SetHost(this);

            try
            {
                root.Load(SceneGraphClock, Dependencies);
            }
            catch (DependencyInjectionException die)
            {
                die.DispatchInfo.Throw();
            }

            //publish bootstrapped scene graph to all threads.
            Root = root;
        }

        #endregion

        #region Config

        protected FrameworkDebugConfigManager DebugConfig { get; private set; }

        protected FrameworkConfigManager Config { get; private set; }

        private InvokeOnDisposal inputPerformanceCollectionPeriod;

        private Bindable<bool> bypassFrontToBackPass;

        private Bindable<GCLatencyMode> activeGCMode;

        private Bindable<FrameSync> frameSyncMode;

        private Bindable<string> ignoredInputHandlers;

        private Bindable<double> cursorSensitivity;
        private readonly Bindable<bool> performanceLogging = new Bindable<bool>();

        private Bindable<WindowMode> windowMode;

        protected virtual void SetupConfig(IDictionary<FrameworkSetting, object> gameDefaults)
        {
            var hostDefaults = new Dictionary<FrameworkSetting, object>
            {
                { FrameworkSetting.WindowMode, Window.WindowMode.Default }
            };

            // merge defaults provided by game into host defaults.
            if (gameDefaults != null)
            {
                foreach (var d in gameDefaults)
                    hostDefaults[d.Key] = d.Value;
            }

            Dependencies.Cache(DebugConfig = new FrameworkDebugConfigManager());
            // TODO: Dependencies.Cache(Config = new FrameworkConfigManager(Storage, hostDefaults));

            windowMode = Config.GetBindable<WindowMode>(FrameworkSetting.WindowMode);

            windowMode.BindValueChanged(mode =>
            {
                if (!Window.SupportedWindowModes.Contains(mode.NewValue))
                    windowMode.SetDefault();
            }, true);

            activeGCMode = DebugConfig.GetBindable<GCLatencyMode>(DebugSetting.ActiveGCMode);
            activeGCMode.ValueChanged += e => { GCSettings.LatencyMode = IsActive.Value ? e.NewValue : GCLatencyMode.Interactive; };

            frameSyncMode = Config.GetBindable<FrameSync>(FrameworkSetting.FrameSync);
            frameSyncMode.ValueChanged += e =>
            {
                float refreshRate = 0; // TODO: DisplayDevice.Default?.RefreshRate ?? 0;
                // For invalid refresh rates let's assume 60 Hz as it is most common.
                if (refreshRate <= 0)
                    refreshRate = 60;

                float drawLimiter = refreshRate;
                float updateLimiter = drawLimiter * 2;

                // TODO: setVSyncMode();

                switch (e.NewValue)
                {
                    case FrameSync.VSync:
                        drawLimiter = int.MaxValue;
                        updateLimiter *= 2;
                        break;

                    case FrameSync.Limit2x:
                        drawLimiter *= 2;
                        updateLimiter *= 2;
                        break;

                    case FrameSync.Limit4x:
                        drawLimiter *= 4;
                        updateLimiter *= 4;
                        break;

                    case FrameSync.Limit8x:
                        drawLimiter *= 8;
                        updateLimiter *= 8;
                        break;

                    case FrameSync.Unlimited:
                        drawLimiter = updateLimiter = int.MaxValue;
                        break;
                }

                if (DrawThread != null) DrawThread.ActiveHz = drawLimiter;
                if (UpdateThread != null) UpdateThread.ActiveHz = updateLimiter;
            };

            ignoredInputHandlers = Config.GetBindable<string>(FrameworkSetting.IgnoredInputHandlers);
            ignoredInputHandlers.ValueChanged += e =>
            {
                var configIgnores = e.NewValue.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s));

                // for now, we always want at least one handler disabled (don't want raw and non-raw mouse at once).
                bool restoreDefaults = !configIgnores.Any();

                if (restoreDefaults)
                {
                    // TODO: resetInputHandlers();
                    // TODO: ignoredInputHandlers.Value = string.Join(" ", AvailableInputHandlers.Where(h => !h.Enabled.Value).Select(h => h.ToString()));
                }
                else
                {
                    // TODO: foreach (var handler in AvailableInputHandlers)
                    // {
                    //     var handlerType = handler.ToString();
                    //     handler.Enabled.Value = configIgnores.All(ch => ch != handlerType);
                    // }
                }
            };

            cursorSensitivity = Config.GetBindable<double>(FrameworkSetting.CursorSensitivity);

            Config.BindWith(FrameworkSetting.PerformanceLogging, performanceLogging);
            performanceLogging.BindValueChanged(logging => threads.ForEach(t => t.Monitor.EnablePerformanceProfiling = logging.NewValue), true);

            bypassFrontToBackPass = DebugConfig.GetBindable<bool>(DebugSetting.BypassFrontToBackPass);
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

        private const int thread_join_timeout = 30000;

        private void stopAllThreads()
        {
            threads.ForEach(t => t.Exit());
            threads.Where(t => t.Running).ForEach(t =>
            {
                if (!t.Thread.Join(thread_join_timeout))
                    Logger.Log($"Thread {t.Name} failed to exit in allocated time ({thread_join_timeout}ms).", LoggingTarget.Runtime, LogLevel.Important);
            });

            // as the input thread isn't actually handled by a thread, the above join does not necessarily mean it has been completed to an exiting state.
            while (!InputThread.Exited)
                InputThread.RunUpdate();
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
