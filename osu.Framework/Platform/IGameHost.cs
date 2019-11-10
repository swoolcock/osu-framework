// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Backends;
using osu.Framework.Backends.Window;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.IO.Stores;
using osu.Framework.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Framework.Platform
{
    public interface IGameHost : IBackendProvider, IDisposable
    {
        #region Events

        /// <summary>
        /// Invoked when the game window is activated. Always invoked from the update thread.
        /// </summary>
        event Action Activated;

        /// <summary>
        /// Invoked when the game window is deactivated. Always invoked from the update thread.
        /// </summary>
        event Action Deactivated;

        event Func<bool> Exiting;
        event Action Exited;

        /// <summary>
        /// An unhandled exception was thrown. Return true to ignore and continue running.
        /// </summary>
        event Func<Exception, bool> ExceptionThrown;

        #endregion

        #region Properties

        string Name { get; }

        bool IsPortableInstallation { get; }

        #endregion

        #region Bindables

        /// <summary>
        /// Whether the <see cref="IWindow"/> is active (in the foreground).
        /// </summary>
        IBindable<bool> IsActive { get; }

        #endregion

        Clipboard GetClipboard();

        Storage GetStorage(string baseName);

        Storage Storage { get; }

        /// <summary>
        /// Requests that a file be opened externally with an associated application, if available.
        /// </summary>
        /// <param name="filename">The absolute path to the file which should be opened.</param>
        void OpenFileExternally(string filename);

        /// <summary>
        /// Create a texture loader store based on an underlying data store.
        /// </summary>
        /// <param name="underlyingStore">The underlying provider of texture data (in arbitrary image formats).</param>
        /// <returns>A texture loader store.</returns>
        IResourceStore<TextureUpload> CreateTextureLoaderStore(IResourceStore<byte[]> underlyingStore);

        /// <summary>
        /// Create a <see cref="VideoDecoder"/> with the given stream. May be overridden by platforms that require a different
        /// decoder implementation.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to decode.</param>
        /// <param name="scheduler">The <see cref="Scheduler"/> to use when scheduling tasks from the decoder thread.</param>
        /// <returns>An instance of <see cref="VideoDecoder"/> initialised with the given stream.</returns>
        VideoDecoder CreateVideoDecoder(Stream stream, Scheduler scheduler);

        void Exit();

        void OpenUrlExternally(string url);

        Task<Image<Rgba32>> TakeScreenshotAsync();

        IEnumerable<KeyBinding> PlatformKeyBindings { get; }

        ITextInputSource GetTextInput(); // TODO: move to IInput

        #region Execution

        ExecutionState ExecutionState { get; }
        DependencyContainer Dependencies { get; }

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
