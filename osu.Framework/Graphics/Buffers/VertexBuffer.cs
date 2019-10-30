// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Buffers;
using osu.Framework.Backends.Graphics;
using osu.Framework.Development;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Vertices;
using osu.Framework.Platform;
using osu.Framework.Statistics;
using SixLabors.Memory;

namespace osu.Framework.Graphics.Buffers
{
    public abstract class VertexBuffer<T> : IDisposable
        where T : struct, IEquatable<T>, IVertex
    {
        protected static readonly int STRIDE = VertexUtils<DepthWrappingVertex<T>>.STRIDE;

        private readonly BufferUsage usage;

        private IMemoryOwner<DepthWrappingVertex<T>> memoryOwner;
        private Memory<DepthWrappingVertex<T>> vertexMemory;

        private bool isInitialised;
        private int vboId;

        protected VertexBuffer(int amountVertices, BufferUsage usage)
        {
            this.usage = usage;

            memoryOwner = SixLabors.ImageSharp.Configuration.Default.MemoryAllocator.Allocate<DepthWrappingVertex<T>>(amountVertices, AllocationOptions.Clean);
            vertexMemory = memoryOwner.Memory;
        }

        /// <summary>
        /// Sets the vertex at a specific index of this <see cref="VertexBuffer{T}"/>.
        /// </summary>
        /// <param name="vertexIndex">The index of the vertex.</param>
        /// <param name="vertex">The vertex.</param>
        /// <returns>Whether the vertex changed.</returns>
        public bool SetVertex(int vertexIndex, T vertex)
        {
            ref var currentVertex = ref vertexMemory.Span[vertexIndex];

            bool isNewVertex = !currentVertex.Vertex.Equals(vertex) || currentVertex.BackbufferDrawDepth != GLWrapper.BackbufferDrawDepth;

            currentVertex.Vertex = vertex;
            currentVertex.BackbufferDrawDepth = GLWrapper.BackbufferDrawDepth;

            return isNewVertex;
        }

        /// <summary>
        /// Gets the number of vertices in this <see cref="VertexBuffer{T}"/>.
        /// </summary>
        public int Size => vertexMemory.Length;

        private NativeMemoryTracker.NativeMemoryLease memoryLease;

        /// <summary>
        /// Initialises this <see cref="VertexBuffer{T}"/>. Guaranteed to be run on the draw thread.
        /// </summary>
        protected virtual void Initialise()
        {
            ThreadSafety.EnsureDrawThread();

            int size = Size * STRIDE;
            vboId = Renderer.Shared.CreateVertexBuffer<DepthWrappingVertex<T>>(usage, (uint)size);
            memoryLease = NativeMemoryTracker.AddMemory(this, size);
        }

        ~VertexBuffer()
        {
            Renderer.Shared.ScheduleDisposal(() => Dispose(false));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected bool IsDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            memoryOwner.Dispose();
            memoryOwner = null;
            vertexMemory = null;

            if (isInitialised)
            {
                Unbind();

                memoryLease?.Dispose();
                Renderer.Shared.DestroyBuffer(vboId);
            }

            IsDisposed = true;
        }

        public virtual void Bind(bool forRendering)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(ToString(), "Can not bind disposed vertex buffers.");

            if (!isInitialised)
            {
                Initialise();
                isInitialised = true;
            }

            Renderer.Shared.BindVertexBuffer<DepthWrappingVertex<T>>(vboId);
        }

        public virtual void Unbind()
        {
        }

        protected virtual int ToElements(int vertices) => vertices;

        protected virtual int ToElementIndex(int vertexIndex) => vertexIndex;

        protected abstract BatchPrimitiveType Type { get; }

        public void Draw()
        {
            DrawRange(0, vertexMemory.Length);
        }

        public void DrawRange(int startIndex, int endIndex)
        {
            Bind(true);

            int amountVertices = endIndex - startIndex;
            Renderer.Shared.DrawIndices(Type, ToElementIndex(startIndex), ToElements(amountVertices));

            Unbind();
        }

        public void Update()
        {
            UpdateRange(0, vertexMemory.Length);
        }

        public void UpdateRange(int startIndex, int endIndex)
        {
            Bind(false);

            int amountVertices = endIndex - startIndex;
            Renderer.Shared.UpdateVertexBuffer(startIndex * STRIDE, (uint)(amountVertices * STRIDE), ref vertexMemory.Span[startIndex]);

            Unbind();

            FrameStatistics.Add(StatisticsCounterType.VerticesUpl, amountVertices);
        }
    }
}
