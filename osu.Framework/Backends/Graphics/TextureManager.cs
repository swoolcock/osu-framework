// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics
{
    public abstract class TextureManager : ITextureManager
    {
        public static ITextureManager Shared { get; internal set; }

        protected IGraphics Graphics;

        internal TextureManager(IGraphics graphics)
        {
            Shared = this;
            Graphics = graphics;
        }

        public virtual Texture CreateTexture(Stream stream, TextureAtlas atlas = null)
        {
            if (stream == null || stream.Length == 0)
                return null;

            try
            {
                var data = new TextureUpload(stream);
                Texture tex = atlas == null ? CreateTexture(data.Width, data.Height) : CreateTexture(atlas.Add(data.Width, data.Height));
                tex.SetData(data);
                return tex;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public abstract ITextureSource CreateTextureSubSource(RectangleI bounds, ITextureSource parent);
        public abstract ITextureSource CreateTextureAtlasSource(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear);
        public abstract ITextureSource CreateTextureSource(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear);
        public abstract ITextureSource CreateTextureAtlasWhiteSource(ITextureSource parent);

        public virtual Texture CreateTexture(ITextureSource source, bool refCount = false) =>
            refCount ? new TextureWithRefCount(source) : new Texture(source);

        public virtual Texture CreateTexture(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear, bool refCount = false) =>
            CreateTexture(CreateTextureSource(width, height, manualMipmaps, filteringMode), refCount);

        public virtual TextureAtlas CreateTextureAtlas(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear) =>
            new TextureAtlas(width, height, manualMipmaps, filteringMode);
    }
}
