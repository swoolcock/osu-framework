// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Framework.Graphics.OpenGL
{
    public struct MaskingInfo : IEquatable<MaskingInfo>
    {
        public RectangleI ScreenSpaceAABB;
        public RectangleF MaskingRect;

        public Quad ConservativeScreenSpaceQuad;

        /// <summary>
        /// This matrix transforms screen space coordinates to masking space (likely the parent
        /// space of the container doing the masking).
        /// It is used by a shader to determine which pixels to discard.
        /// </summary>
        public Matrix3 ToMaskingSpace;

        public float CornerRadius;

        public float BorderThickness;
        public SRGBColour BorderColour;

        public float BlendRange;
        public float AlphaExponent;

        public Vector2 EdgeOffset;

        public bool Hollow;
        public float HollowCornerRadius;

        public bool Equals(MaskingInfo other) =>
            ScreenSpaceAABB == other.ScreenSpaceAABB &&
            MaskingRect == other.MaskingRect &&
            ToMaskingSpace == other.ToMaskingSpace &&
            CornerRadius == other.CornerRadius &&
            BorderThickness == other.BorderThickness &&
            BorderColour.Equals(other.BorderColour) &&
            BlendRange == other.BlendRange &&
            AlphaExponent == other.AlphaExponent &&
            EdgeOffset == other.EdgeOffset &&
            Hollow == other.Hollow &&
            HollowCornerRadius == other.HollowCornerRadius;
    }
}
