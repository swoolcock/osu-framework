// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.Vertices;

namespace osu.Framework.Graphics.Buffers
{
    internal static class LinearIndexData
    {
        public static int EBO_ID = -1;
        public static int MaxAmountIndices;
    }

    /// <summary>
    /// This type of vertex buffer lets the ith vertex be referenced by the ith index.
    /// </summary>
    public class LinearVertexBuffer<T> : VertexBuffer<T>
        where T : struct, IEquatable<T>, IVertex
    {
        private readonly int amountVertices;

        internal LinearVertexBuffer(int amountVertices, BatchPrimitiveType type, BufferUsage usage)
            : base(amountVertices, usage)
        {
            this.amountVertices = amountVertices;
            Type = type;
        }

        protected override void Initialise()
        {
            base.Initialise();

            if (amountVertices > LinearIndexData.MaxAmountIndices)
            {
                ushort[] indices = new ushort[amountVertices];

                for (ushort i = 0; i < amountVertices; i++)
                    indices[i] = i;

                int size = amountVertices * sizeof(ushort);
                LinearIndexData.EBO_ID = Renderer.Shared.CreateIndexBuffer((uint)size, LinearIndexData.EBO_ID);
                Renderer.Shared.BindIndexBuffer(LinearIndexData.EBO_ID);
                Renderer.Shared.UpdateIndexBuffer(BufferUsage.Static, indices, (uint)size);
                LinearIndexData.MaxAmountIndices = amountVertices;
            }
        }

        public override void Bind(bool forRendering)
        {
            base.Bind(forRendering);

            if (forRendering && LinearIndexData.EBO_ID >= 0)
                Renderer.Shared.BindIndexBuffer(LinearIndexData.EBO_ID);
        }

        protected override BatchPrimitiveType Type { get; }
    }
}
