// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Shaders;
using osuTK;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKShader : Shader<OsuTKShaderPart>
    {
        internal bool IsBound;
        private int programID = -1;
        private IUniform[] uniformsArray;

        internal OsuTKShader(string name, List<OsuTKShaderPart> parts)
            : base(name, parts)
        {
            GLWrapper.EnqueueShaderCompile(this);
        }

        internal void Compile()
        {
            Parts.RemoveAll(p => p == null);
            Uniforms.Clear();
            uniformsArray = null;

            if (Parts.Count == 0)
                return;

            programID = GL.CreateProgram();

            foreach (OsuTKShaderPart p in Parts)
            {
                if (!p.Compiled) p.Compile();
                GL.AttachShader(this, p);

                foreach (ShaderInputInfo input in p.ShaderInputs)
                    GL.BindAttribLocation(this, input.Location, input.Name);
            }

            GL.LinkProgram(this);
            GL.GetProgram(this, GetProgramParameterName.LinkStatus, out int linkResult);

            foreach (var part in Parts)
                GL.DetachShader(this, part);

            IsLoaded = linkResult == 1;

            if (!IsLoaded)
                throw new ProgramLinkingFailedException(Name, GL.GetProgramInfoLog(this));

            // Obtain all the shader uniforms
            GL.GetProgram(this, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            uniformsArray = new IUniform[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(this, i, 100, out _, out _, out ActiveUniformType type, out string uniformName);

                IUniform createUniform<T>(string name)
                    where T : struct
                {
                    int location = GL.GetUniformLocation(this, name);

                    if (GlobalPropertyManager.CheckGlobalExists(name)) return new OsuTKGlobalUniform<T>(this, name, location);

                    return new OsuTKUniform<T>(this, name, location);
                }

                IUniform uniform;

                switch (type)
                {
                    case ActiveUniformType.Bool:
                        uniform = createUniform<bool>(uniformName);
                        break;

                    case ActiveUniformType.Float:
                        uniform = createUniform<float>(uniformName);
                        break;

                    case ActiveUniformType.Int:
                        uniform = createUniform<int>(uniformName);
                        break;

                    case ActiveUniformType.FloatMat3:
                        uniform = createUniform<Matrix3>(uniformName);
                        break;

                    case ActiveUniformType.FloatMat4:
                        uniform = createUniform<Matrix4>(uniformName);
                        break;

                    case ActiveUniformType.FloatVec2:
                        uniform = createUniform<Vector2>(uniformName);
                        break;

                    case ActiveUniformType.FloatVec3:
                        uniform = createUniform<Vector3>(uniformName);
                        break;

                    case ActiveUniformType.FloatVec4:
                        uniform = createUniform<Vector4>(uniformName);
                        break;

                    case ActiveUniformType.Sampler2D:
                        uniform = createUniform<int>(uniformName);
                        break;

                    default:
                        continue;
                }

                uniformsArray[i] = uniform;
                Uniforms.Add(uniformName, uniformsArray[i]);
            }

            GlobalPropertyManager.Register(this);
        }

        internal void EnsureLoaded()
        {
            if (!IsLoaded)
                Compile();
        }

        public override void Bind()
        {
            if (IsBound)
                return;

            EnsureLoaded();

            GLWrapper.UseProgram(this);

            foreach (var uniform in uniformsArray)
                uniform?.Update();

            IsBound = true;
        }

        public override void Unbind()
        {
            if (!IsBound)
                return;

            GLWrapper.UseProgram(null);

            IsBound = false;
        }

        public override string ToString() => $@"{Name} Shader (Compiled: {programID != -1})";

        /// <summary>
        /// Returns a uniform from the shader.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <returns>Returns a base uniform.</returns>
        public override Uniform<T> GetUniform<T>(string name)
        {
            EnsureLoaded();

            return (Uniform<T>)Uniforms[name];
        }

        public static implicit operator int(OsuTKShader shader) => shader.programID;
    }
}
