// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics.Shaders;
using osu.Framework.IO.Stores;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics.OsuTK
{
    internal class OsuTKShaderManager : ShaderManager<OsuTKShader, OsuTKShaderPart>
    {
        public OsuTKShaderManager(ResourceStore<byte[]> store)
            : base(store)
        {
        }

        internal override OsuTKShader CreateShader(string name, List<OsuTKShaderPart> parts) => new OsuTKShader(name, parts);

        internal override OsuTKShaderPart CreateShaderPart(string name, byte[] data, ShaderType type) => new OsuTKShaderPart(name, data, type, this);
    }
}
