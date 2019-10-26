// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Buffers;
using osu.Framework.Graphics.Vertices;
using osuTK.Graphics.ES30;

namespace osu.Framework.Graphics.Batches
{
    public class LinearBatch<T> : VertexBatch<T>
        where T : struct, IEquatable<T>, IVertex
    {
        private readonly BatchPrimitiveType type;

        public LinearBatch(int size, int maxBuffers, BatchPrimitiveType type)
            : base(size, maxBuffers)
        {
            this.type = type;
        }

        protected override VertexBuffer<T> CreateVertexBuffer() => new LinearVertexBuffer<T>(Size, type, BufferUsageHint.DynamicDraw);
    }
}
