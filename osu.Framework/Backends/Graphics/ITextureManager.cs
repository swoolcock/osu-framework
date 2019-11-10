// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics
{
    public interface ITextureManager
    {
        Texture CreateTexture(ITextureSource source, bool refCount = false);
        Texture CreateTexture(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear, bool refCount = false);
        TextureAtlas CreateTextureAtlas(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear);
        Texture CreateTexture(Stream stream, TextureAtlas atlas = null);
        ITextureSource CreateTextureSubSource(RectangleI bounds, ITextureSource parent);
        ITextureSource CreateTextureAtlasSource(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear);
        ITextureSource CreateTextureSource(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear);
        ITextureSource CreateTextureAtlasWhiteSource(ITextureSource parent);
    }
}
