// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Graphics.OsuTK;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.Primitives;
using osuTK;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Vertices;
using RectangleF = osu.Framework.Graphics.Primitives.RectangleF;

namespace osu.Framework.Graphics.Textures
{
    public class Texture : IDisposable
    {
        public const int MAX_MIPMAP_LEVELS = 3;

        // in case no other textures are used in the project, create a new atlas as a fallback source for the white pixel area (used to draw boxes etc.)
        private static readonly Lazy<TextureWhitePixel> white_pixel = new Lazy<TextureWhitePixel>(() =>
            new TextureAtlas(TextureAtlas.WHITE_PIXEL_SIZE + TextureAtlas.PADDING, TextureAtlas.WHITE_PIXEL_SIZE + TextureAtlas.PADDING, true).WhitePixel);

        public static Texture WhitePixel => white_pixel.Value;

        public virtual ITextureSource Source { get; }

        public string Filename;
        public string AssetName;

        /// <summary>
        /// At what multiple of our expected resolution is our underlying texture?
        /// </summary>
        public float ScaleAdjust = 1;

        public float DisplayWidth => Width / ScaleAdjust;
        public float DisplayHeight => Height / ScaleAdjust;

        /// <summary>
        /// Create a new texture.
        /// </summary>
        /// <param name="textureGl">The GL texture.</param>
        public Texture(ITextureSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public int Width
        {
            get => Source.Width;
            set => Source.Width = value;
        }

        public int Height
        {
            get => Source.Height;
            set => Source.Height = value;
        }

        public Vector2 Size => new Vector2(Width, Height);

        public bool PrepareDraw() => Source?.PrepareDraw() ?? false;

        /// <summary>
        /// Queue a <see cref="TextureUpload"/> to be uploaded on the draw thread.
        /// The provided upload will be disposed after the upload is completed.
        /// </summary>
        /// <param name="upload"></param>
        public void SetData(ITextureUpload upload) => Source?.SetData(upload);

        protected virtual RectangleF TextureBounds(RectangleF? textureRect = null)
        {
            RectangleF texRect = textureRect ?? new RectangleF(0, 0, DisplayWidth, DisplayHeight);

            if (ScaleAdjust != 1)
            {
                texRect.Width *= ScaleAdjust;
                texRect.Height *= ScaleAdjust;
                texRect.X *= ScaleAdjust;
                texRect.Y *= ScaleAdjust;
            }

            return texRect;
        }

        public RectangleF GetTextureRect(RectangleF? textureRect = null) => Source.GetTextureRect(TextureBounds(textureRect));

        /// <summary>
        /// Draws a triangle to the screen.
        /// </summary>
        /// <param name="vertexTriangle">The triangle to draw.</param>
        /// <param name="drawColour">The vertex colour.</param>
        /// <param name="textureRect">The texture rectangle.</param>
        /// <param name="vertexAction">An action that adds vertices to a <see cref="VertexBatch{T}"/>.</param>
        /// <param name="inflationPercentage">The percentage amount that <see cref="textureRect"/> should be inflated.</param>
        public void DrawTriangle(Triangle vertexTriangle, ColourInfo drawColour, RectangleF? textureRect = null, Action<TexturedVertex2D> vertexAction = null,
                                 Vector2? inflationPercentage = null)
        {
            if (!PrepareDraw()) return;

            Source?.DrawTriangle(vertexTriangle, drawColour, TextureBounds(textureRect), vertexAction, inflationPercentage);
        }

        /// <summary>
        /// Draws a quad to the screen.
        /// </summary>
        /// <param name="vertexQuad">The quad to draw.</param>
        /// <param name="drawColour">The vertex colour.</param>
        /// <param name="textureRect">The texture rectangle.</param>
        /// <param name="vertexAction">An action that adds vertices to a <see cref="VertexBatch{T}"/>.</param>
        /// <param name="inflationPercentage">The percentage amount that <see cref="textureRect"/> should be inflated.</param>
        /// <param name="blendRangeOverride">The range over which the edges of the <see cref="textureRect"/> should be blended.</param>
        public void DrawQuad(Quad vertexQuad, ColourInfo drawColour, RectangleF? textureRect = null, Action<TexturedVertex2D> vertexAction = null, Vector2? inflationPercentage = null,
                             Vector2? blendRangeOverride = null)
        {
            if (!PrepareDraw()) return;

            Source?.DrawQuad(vertexQuad, drawColour, TextureBounds(textureRect), vertexAction, inflationPercentage, blendRangeOverride);
        }

        public override string ToString() => $@"{AssetName} ({Width}, {Height})";

        /// <summary>
        /// Whether <see cref="TextureGL"/> is in a usable state.
        /// </summary>
        public virtual bool Available => Source?.Available ?? false;

        #region Disposal

        // Intentionally no finalizer implementation as our disposal is NOOP. Finalizer is implemented in TextureWithRefCount usage.

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }

        #endregion
    }
}
