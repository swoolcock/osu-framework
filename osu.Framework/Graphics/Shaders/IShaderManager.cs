// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Graphics.Shaders
{
    public interface IShaderManager
    {
        byte[] LoadRaw(string name);

        IShader Load(string vertex, string fragment, bool continuousCompilation = false);
    }
}
