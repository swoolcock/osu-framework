// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osuTK.Graphics.ES30;

namespace osu.Framework.Graphics.Batches
{
    public enum BatchPrimitiveType
    {
        Points,
        Lines,
        LineStrip,
        Triangles,
        TriangleStrip,
    }

    public static class BatchPrimitiveTypeExtensions
    {
        public static PrimitiveType ToOpenGL(this BatchPrimitiveType type)
        {
            switch (type)
            {
                case BatchPrimitiveType.Lines:
                    return PrimitiveType.Lines;

                case BatchPrimitiveType.LineStrip:
                    return PrimitiveType.LineStrip;

                case BatchPrimitiveType.Triangles:
                    return PrimitiveType.Triangles;

                case BatchPrimitiveType.TriangleStrip:
                    return PrimitiveType.TriangleStrip;

                case BatchPrimitiveType.Points:
                    return PrimitiveType.Points;

                default:
                    throw new ArgumentException(nameof(type));
            }
        }
    }
}
