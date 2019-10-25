// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK.Graphics.ES30;

namespace osu.Framework.Graphics.Shaders
{
    public abstract class ShaderPart
    {
        internal readonly string Name;
        internal readonly ShaderType Type;
        internal readonly IShaderManager Manager;

        protected ShaderPart(string name, ShaderType type, IShaderManager manager)
        {
            Name = name;
            Type = type;
            Manager = manager;
        }
    }
}
