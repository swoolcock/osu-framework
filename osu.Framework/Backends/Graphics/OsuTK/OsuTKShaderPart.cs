// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using osu.Framework.Graphics.Shaders;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKShaderPart : ShaderPart
    {
        internal const string SHADER_ATTRIBUTE_PATTERN = "^\\s*(?>attribute|in)\\s+(?:(?:lowp|mediump|highp)\\s+)?\\w+\\s+(\\w+)";

        internal List<ShaderInputInfo> ShaderInputs = new List<ShaderInputInfo>();

        internal bool HasCode;
        internal bool Compiled;

        private bool isVertexShader => Type == ShaderType.VertexShader || Type == ShaderType.VertexShaderArb;

        private int partID = -1;

        private int lastShaderInputIndex;

        private readonly List<string> shaderCodes = new List<string>();

        private readonly Regex includeRegex = new Regex("^\\s*#\\s*include\\s+[\"<](.*)[\">]");
        private readonly Regex shaderInputRegex = new Regex(SHADER_ATTRIBUTE_PATTERN);

        internal OsuTKShaderPart(string name, byte[] data, ShaderType type, OsuTKShaderManager manager)
            : base(name, type, manager)
        {
            shaderCodes.Add(loadFile(data, true));
            shaderCodes.RemoveAll(string.IsNullOrEmpty);

            if (shaderCodes.Count == 0)
                return;

            HasCode = true;
        }

        private string loadFile(byte[] bytes, bool mainFile)
        {
            if (bytes == null)
                return null;

            using (MemoryStream ms = new MemoryStream(bytes))
            using (StreamReader sr = new StreamReader(ms))
            {
                string code = string.Empty;

                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("#version")) // the version directive has to appear before anything else in the shader
                    {
                        shaderCodes.Add(line);
                        continue;
                    }

                    Match includeMatch = includeRegex.Match(line);

                    if (includeMatch.Success)
                    {
                        string includeName = includeMatch.Groups[1].Value.Trim();

                        //#if DEBUG
                        //                        byte[] rawData = null;
                        //                        if (File.Exists(includeName))
                        //                            rawData = File.ReadAllBytes(includeName);
                        //#endif
                        code += loadFile(Manager.LoadRaw(includeName), false) + '\n';
                    }
                    else
                        code += line + '\n';

                    if (Type == ShaderType.VertexShader || Type == ShaderType.VertexShaderArb)
                    {
                        Match inputMatch = shaderInputRegex.Match(line);

                        if (inputMatch.Success)
                        {
                            ShaderInputs.Add(new ShaderInputInfo
                            {
                                Location = lastShaderInputIndex++,
                                Name = inputMatch.Groups[1].Value.Trim()
                            });
                        }
                    }
                }

                if (mainFile)
                {
                    code = loadFile(Manager.LoadRaw("sh_Precision_Internal.h"), false) + "\n" + code;

                    if (isVertexShader)
                    {
                        string realMainName = "real_main_" + Guid.NewGuid().ToString("N");

                        string backbufferCode = loadFile(Manager.LoadRaw("sh_Backbuffer_Internal.h"), false);

                        backbufferCode = backbufferCode.Replace("{{ real_main }}", realMainName);
                        code = Regex.Replace(code, @"void main\((.*)\)", $"void {realMainName}()") + backbufferCode + '\n';
                    }
                }

                return code;
            }
        }

        internal bool Compile()
        {
            if (!HasCode)
                return false;

            if (partID == -1)
                partID = GL.CreateShader(Type);

            int[] codeLengths = new int[shaderCodes.Count];
            for (int i = 0; i < shaderCodes.Count; i++)
                codeLengths[i] = shaderCodes[i].Length;

            GL.ShaderSource(this, shaderCodes.Count, shaderCodes.ToArray(), codeLengths);
            GL.CompileShader(this);

            GL.GetShader(this, ShaderParameter.CompileStatus, out int compileResult);
            Compiled = compileResult == 1;

            if (!Compiled)
                throw new PartCompilationFailedException(Name, GL.GetShaderInfoLog(this));

            return Compiled;
        }

        public static implicit operator int(OsuTKShaderPart program) => program.partID;

        private void delete()
        {
            if (partID == -1) return;

            GL.DeleteShader(this);
            Compiled = false;
            partID = -1;
        }
    }
}
