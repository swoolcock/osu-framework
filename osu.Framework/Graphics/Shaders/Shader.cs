// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;

namespace osu.Framework.Graphics.Shaders
{
    public abstract class Shader<P> : IShader where P : ShaderPart
    {
        public abstract void Bind();
        public abstract void Unbind();
        public virtual bool IsLoaded { get; protected set; }

        public abstract Uniform<T> GetUniform<T>(string name) where T : struct;

        protected string Name { get; }

        protected List<P> Parts { get; }

        public Dictionary<string, IUniform> Uniforms { get; } = new Dictionary<string, IUniform>();

        internal Shader(string name, List<P> parts)
        {
            Name = name;
            Parts = parts;
        }
    }

    public class PartCompilationFailedException : Exception
    {
        public PartCompilationFailedException(string partName, string log)
            : base($"A {typeof(IShader)} failed to compile: {partName}:\n{log.Trim()}")
        {
        }
    }

    public class ProgramLinkingFailedException : Exception
    {
        public ProgramLinkingFailedException(string programName, string log)
            : base($"A {typeof(IShader)} failed to link: {programName}:\n{log.Trim()}")
        {
        }
    }
}
