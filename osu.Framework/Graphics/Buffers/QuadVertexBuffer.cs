// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Backends.Graphics;
using osu.Framework.Backends.Graphics.OsuTK;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.Vertices;

namespace osu.Framework.Graphics.Buffers
{
    internal static class QuadIndexData
    {
        public static int EboID = -1;
        public static int MaxAmountIndices;
    }

    public class QuadVertexBuffer<T> : VertexBuffer<T>
        where T : struct, IEquatable<T>, IVertex
    {
        private readonly int amountQuads;

        internal QuadVertexBuffer(int amountQuads, BufferUsage usage)
            : base(amountQuads * TextureGLSingle.VERTICES_PER_QUAD, usage)
        {
            this.amountQuads = amountQuads;
        }

        protected override void Initialise()
        {
            base.Initialise();

            int amountIndices = amountQuads * 6;

            if (amountIndices > QuadIndexData.MaxAmountIndices)
            {
                ushort[] indices = new ushort[amountIndices];

                for (ushort i = 0, j = 0; j < amountIndices; i += TextureGLSingle.VERTICES_PER_QUAD, j += 6)
                {
                    indices[j] = i;
                    indices[j + 1] = (ushort)(i + 1);
                    indices[j + 2] = (ushort)(i + 3);
                    indices[j + 3] = (ushort)(i + 2);
                    indices[j + 4] = (ushort)(i + 3);
                    indices[j + 5] = (ushort)(i + 1);
                }

                int size = amountIndices * sizeof(ushort);
                QuadIndexData.EboID = Renderer.Shared.CreateIndexBuffer((uint)size, QuadIndexData.EboID);
                Renderer.Shared.BindIndexBuffer(QuadIndexData.EboID);
                Renderer.Shared.UpdateIndexBuffer(BufferUsage.Static, indices, (uint)size);
                QuadIndexData.MaxAmountIndices = amountIndices;
            }
        }

        public override void Bind(bool forRendering)
        {
            base.Bind(forRendering);

            if (forRendering && QuadIndexData.EboID >= 0)
                Renderer.Shared.BindIndexBuffer(QuadIndexData.EboID);
        }

        protected override int ToElements(int vertices) => 3 * vertices / 2;

        protected override int ToElementIndex(int vertexIndex) => 3 * vertexIndex / 2;

        protected override BatchPrimitiveType Type => BatchPrimitiveType.Triangles;
    }
}
