// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Buffers;
using osu.Framework.Graphics.Vertices;
using BufferUsage = osu.Framework.Graphics.Buffers.BufferUsage;

namespace osu.Framework.Graphics.Batches
{
    public class QuadBatch<T> : VertexBatch<T>
        where T : struct, IEquatable<T>, IVertex
    {
        public QuadBatch(int size, int maxBuffers)
            : base(size, maxBuffers)
        {
        }

        protected override VertexBuffer<T> CreateVertexBuffer() => new QuadVertexBuffer<T>(Size, BufferUsage.Dynamic);
    }
}
