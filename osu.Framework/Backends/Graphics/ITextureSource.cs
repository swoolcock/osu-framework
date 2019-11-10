// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Vertices;
using osuTK;

namespace osu.Framework.Backends.Graphics
{
    public interface ITextureSource : IDisposable
    {
        int Width { get; set; }
        int Height { get; set; }
        bool Available { get; }

        void SetData(ITextureUpload upload);
        RectangleF GetTextureRect(RectangleF? textureRect = null);

        bool PrepareDraw();

        bool Upload();

        void FlushUploads();

        void DrawTriangle(Triangle vertexTriangle, ColourInfo drawColour, RectangleF? textureRect = null, Action<TexturedVertex2D> vertexAction = null,
                          Vector2? inflationPercentage = null);

        void DrawQuad(Quad vertexQuad, ColourInfo drawColour, RectangleF? textureRect = null, Action<TexturedVertex2D> vertexAction = null, Vector2? inflationPercentage = null, Vector2? blendRangeOverride = null);
    }
}
