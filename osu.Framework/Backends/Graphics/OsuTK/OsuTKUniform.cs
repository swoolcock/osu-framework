// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Shaders;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKUniform<T> : Uniform<T>, IOsuTKUniform
        where T : struct
    {
        public new OsuTKShader Owner => (OsuTKShader)base.Owner;

        public int Location { get; }

        public OsuTKUniform(IShader owner, string name, int uniformLocation)
            : base(owner, name)
        {
            Location = uniformLocation;
        }

        protected override void UpdateUniform() => GLWrapper.SetUniform(this);
        protected override bool CanUpdate() => Owner.IsBound;
    }
}
