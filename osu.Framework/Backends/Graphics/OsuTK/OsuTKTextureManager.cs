// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Primitives;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKTextureManager : TextureManager
    {
        public new OsuTKGraphicsBackend Graphics => (OsuTKGraphicsBackend)base.Graphics;

        public OsuTKTextureManager(OsuTKGraphicsBackend graphics)
            : base(graphics)
        {
        }

        public override ITextureSource CreateTextureSource(int width, int height, bool manualMipmaps = false,
                                                           All filteringMode = All.Linear) => new TextureGLSingle(width, height, manualMipmaps, filteringMode);

        public override ITextureSource CreateTextureSubSource(RectangleI bounds, ITextureSource parent) => new TextureGLSub(bounds, parent as TextureGLSingle);

        public override ITextureSource CreateTextureAtlasSource(int width, int height, bool manualMipmaps = false, All filteringMode = All.Linear) =>
            new TextureGLAtlas(width, height, manualMipmaps, filteringMode);

        public override ITextureSource CreateTextureAtlasWhiteSource(ITextureSource parent) => new TextureGLAtlasWhite(parent as TextureGLSingle);
    }
}
