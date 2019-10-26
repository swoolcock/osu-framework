// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using osu.Framework.IO.Stores;
using osuTK.Graphics.ES30;

namespace osu.Framework.Graphics.Shaders
{
    public abstract class ShaderManager<S, P> : IShaderManager
        where S : Shader<P>
        where P : ShaderPart
    {
        private const string shader_prefix = @"sh_";

        private readonly ConcurrentDictionary<string, P> partCache = new ConcurrentDictionary<string, P>();
        private readonly ConcurrentDictionary<(string, string), S> shaderCache = new ConcurrentDictionary<(string, string), S>();

        protected readonly ResourceStore<byte[]> Store;

        protected ShaderManager(ResourceStore<byte[]> store)
        {
            Store = store;
        }

        internal abstract S CreateShader(string name, List<P> parts);
        internal abstract P CreateShaderPart(string name, byte[] data, ShaderType type);

        public byte[] LoadRaw(string name) => Store.Get(name);

        private string getFileEnding(ShaderType type)
        {
            switch (type)
            {
                case ShaderType.FragmentShader:
                    return @".fs";

                case ShaderType.VertexShader:
                    return @".vs";
            }

            return string.Empty;
        }

        private string ensureValidName(string name, ShaderType type)
        {
            string ending = getFileEnding(type);
            if (!name.StartsWith(shader_prefix, StringComparison.Ordinal))
                name = shader_prefix + name;
            if (name.EndsWith(ending, StringComparison.Ordinal))
                return name;

            return name + ending;
        }

        private P createShaderPart(string name, ShaderType type, bool bypassCache = false)
        {
            name = ensureValidName(name, type);

            if (!bypassCache && partCache.TryGetValue(name, out P part))
                return part;

            byte[] rawData = LoadRaw(name);

            part = CreateShaderPart(name, rawData, type);

            //cache even on failure so we don't try and fail every time.
            partCache[name] = part;
            return part;
        }

        public IShader Load(string vertex, string fragment, bool continuousCompilation = false)
        {
            var tuple = (vertex, fragment);

            if (shaderCache.TryGetValue(tuple, out S shader))
                return shader;

            return shaderCache[tuple] = CreateShader($"{vertex}/{fragment}", new List<P>
            {
                createShaderPart(vertex, ShaderType.VertexShader),
                createShaderPart(fragment, ShaderType.FragmentShader)
            });
        }
    }

    public static class VertexShaderDescriptor
    {
        public const string TEXTURE_2 = "Texture2D";
        public const string TEXTURE_3 = "Texture3D";
        public const string POSITION = "Position";
        public const string COLOUR = "Colour";
    }

    public static class FragmentShaderDescriptor
    {
        public const string TEXTURE = "Texture";
        public const string TEXTURE_ROUNDED = "TextureRounded";
        public const string COLOUR = "Colour";
        public const string COLOUR_ROUNDED = "ColourRounded";
        public const string GLOW = "Glow";
        public const string BLUR = "Blur";
    }
}
