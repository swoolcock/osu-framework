﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

// #define VELDRID

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Backends.Audio;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Input;
using osu.Framework.Backends.Video;
using osu.Framework.Backends.Window;
using osu.Framework.Input;
using osuTK;

namespace osu.Framework.Platform
{
    public abstract class DesktopGameHost : GameHost
    {
        private const int default_window_width = 1366;
        private const int default_window_height = 768;

        private TcpIpcProvider ipcProvider;
        private readonly bool bindIPCPort;
        private Thread ipcThread;

        protected override IAudio CreateAudio() => new BassAudioBackend();
        protected override IVideo CreateVideo() => new FfmpegVideoBackend();

#if VELDRID
        protected override IGraphics CreateGraphics() => new Sdl2GraphicsBackend();
        protected override IInput CreateInput() => new VeldridInputBackend();
        protected override IWindow CreateWindow() => new VeldridWindowBackend();
#else
        protected override IGraphics CreateGraphics() => new OsuTKGraphicsBackend();
        protected override IInput CreateInput() => new OsuTKInputBackend();
        protected override IWindow CreateWindow() => new OsuTKWindowBackend(default_window_width, default_window_height);
#endif

        protected DesktopGameHost(string gameName = @"", bool bindIPCPort = false, ToolkitOptions toolkitOptions = default, bool portableInstallation = false)
            : base(gameName)
        {
            this.bindIPCPort = bindIPCPort;
            IsPortableInstallation = portableInstallation;
        }

        protected override void SetupForRun()
        {
            //todo: yeah.
            Architecture.SetIncludePath();

            if (bindIPCPort)
                startIPC();

            base.SetupForRun();
        }

        private void startIPC()
        {
            Debug.Assert(ipcProvider == null);

            ipcProvider = new TcpIpcProvider();
            IsPrimaryInstance = ipcProvider.Bind();

            if (IsPrimaryInstance)
            {
                ipcProvider.MessageReceived += OnMessageReceived;

                ipcThread = new Thread(() => ipcProvider.StartAsync().Wait())
                {
                    Name = "IPC",
                    IsBackground = true
                };

                ipcThread.Start();
            }
        }

        public override void OpenFileExternally(string filename) => openUsingShellExecute(filename);

        public override void OpenUrlExternally(string url) => openUsingShellExecute(url);

        private void openUsingShellExecute(string path) => Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true //see https://github.com/dotnet/corefx/issues/10361
        });

        public override ITextInputSource GetTextInput() => new GameWindowTextInput(Input);

        public override Task SendMessageAsync(IpcMessage message) => ipcProvider.SendMessageAsync(message);

        protected override void Dispose(bool isDisposing)
        {
            ipcProvider?.Dispose();
            ipcThread?.Join(50);
            base.Dispose(isDisposing);
        }
    }
}
