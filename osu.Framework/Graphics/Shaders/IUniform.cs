// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Graphics.Shaders
{
    /// <summary>
    /// Represents an updateable shader uniform.
    /// </summary>
    public interface IUniform
    {
        void Update();
        IShader Owner { get; }
        string Name { get; }
    }
}
