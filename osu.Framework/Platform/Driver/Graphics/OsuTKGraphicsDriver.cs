// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Logging;
using osu.Framework.Platform.Driver.Window;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace osu.Framework.Platform.Driver.Graphics
{
    /// <summary>
    /// Concrete implementation of <see cref="IGraphicsDriver"/> that uses osuTK's OpenGL context and calls.
    /// </summary>
    public class OsuTKGraphicsDriver : GraphicsDriver
    {
        private IGraphicsContext context;

        internal Version GLVersion { get; private set; }

        internal Version GLSLVersion { get; private set; }

        internal bool IsEmbedded { get; private set; }

        public override void Initialise(IDriverProvider provider)
        {
            if (!(provider.Window is OsuTKWindowDriver window))
                throw new Exception($"{typeof(OsuTKGraphicsDriver)} requires a matching {typeof(OsuTKWindowDriver)}");

            if (window.Implementation is osuTK.GameWindow impl)
                context = impl.Context;

            window.Implementation.MakeCurrent();

            string version = GL.GetString(StringName.Version);
            string versionNumberSubstring = getVersionNumberSubstring(version);

            GLVersion = new Version(versionNumberSubstring);

            // As defined by https://www.khronos.org/registry/OpenGL-Refpages/es2.0/xhtml/glGetString.xml
            IsEmbedded = version.Contains("OpenGL ES");

            version = GL.GetString(StringName.ShadingLanguageVersion);

            if (!string.IsNullOrEmpty(version))
            {
                try
                {
                    GLSLVersion = new Version(versionNumberSubstring);
                }
                catch (Exception e)
                {
                    Logger.Error(e, $@"couldn't set GLSL version using string '{version}'");
                }
            }

            if (GLSLVersion == null)
                GLSLVersion = new Version();

            Logger.Log($@"GL Initialized
                        GL Version:                 {GL.GetString(StringName.Version)}
                        GL Renderer:                {GL.GetString(StringName.Renderer)}
                        GL Shader Language version: {GL.GetString(StringName.ShadingLanguageVersion)}
                        GL Vendor:                  {GL.GetString(StringName.Vendor)}
                        GL Extensions:              {GL.GetString(StringName.Extensions)}");

            window.Implementation.MakeCurrent();
        }

        private string getVersionNumberSubstring(string version)
        {
            string result = version.Split(' ').FirstOrDefault(s => char.IsDigit(s, 0));
            if (result != null) return result;

            throw new ArgumentException(nameof(version));
        }
    }
}
