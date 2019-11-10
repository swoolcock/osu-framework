// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Backends.Graphics;

namespace osu.Framework.Graphics.Textures
{
    /// <summary>
    /// A texture which updates the reference count of the underlying <see cref="ITextureSource"/> on ctor and disposal.
    /// </summary>
    public class TextureWithRefCount : Texture
    {
        public TextureWithRefCount(ITextureSource source)
            : base(source)
        {
            if (source is IReferenceCounted rc)
                rc.Reference();
        }

        internal int ReferenceCount => (base.Source as IReferenceCounted)?.ReferenceCount ?? 0;

        public sealed override ITextureSource Source
        {
            get
            {
                var tex = base.Source;
                if (tex is IReferenceCounted rc && rc.ReferenceCount <= 0)
                    throw new InvalidOperationException($"Attempting to access a {nameof(TextureWithRefCount)}'s underlying texture after all references are lost.");

                return tex;
            }
        }

        // The base property references TextureGL, but doing so may throw an exception (above)
        public sealed override bool Available => base.Source?.Available ?? false;

        #region Disposal

        ~TextureWithRefCount()
        {
            // Finalizer implemented here rather than Texture to avoid GC overhead.
            Dispose(false);
        }

        private bool isDisposed;

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (isDisposed)
                return;

            isDisposed = true;

            Renderer.Shared.ScheduleDisposal(() => (base.Source as IReferenceCounted)?.Dereference());
        }

        #endregion
    }
}
