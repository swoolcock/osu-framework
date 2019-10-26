// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Backends.Graphics.OsuTK;
using osu.Framework.Backends.Window;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace osu.Framework.Backends.Graphics
{
    /// <summary>
    /// Concrete implementation of <see cref="IGraphicsBackend"/> that uses osuTK's OpenGL context and calls.
    /// </summary>
    public class OsuTKGraphicsBackend : GraphicsBackend
    {
        private IGraphicsContext context;

        private OsuTKWindowBackend windowBackend;

        internal Version GLVersion { get; private set; }

        internal Version GLSLVersion { get; private set; }

        internal bool IsEmbedded { get; private set; }

        public override void Initialise(IGameHost host)
        {
            if (!(host.Window is OsuTKWindowBackend window))
                throw new Exception($"{nameof(OsuTKGraphicsBackend)} requires a corresponding {nameof(OsuTKWindowBackend)}");

            windowBackend = window;

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

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }

        private string getVersionNumberSubstring(string version)
        {
            string result = version.Split(' ').FirstOrDefault(s => char.IsDigit(s, 0));
            if (result != null) return result;

            throw new ArgumentException(nameof(version));
        }

        public override IShaderManager CreateShaderManager(ResourceStore<byte[]> store) => new OsuTKShaderManager(store);

        public override void ResetState() => GLWrapper.Reset(new Vector2(windowBackend.InternalSize.Value.Width, windowBackend.InternalSize.Value.Height));

        public override void SetBlend(BlendingParameters blendingParameters) => GLWrapper.SetBlend(blendingParameters);

        public override void SetDrawDepth(float drawDepth) => GLWrapper.SetDrawDepth(drawDepth);

        public override MaskingInfo CurrentMaskingInfo => GLWrapper.CurrentMaskingInfo;
        public override RectangleI Viewport => GLWrapper.Viewport;
        public override RectangleF Ortho => GLWrapper.Ortho;
        public override Matrix4 ProjectionMatrix => GLWrapper.ProjectionMatrix;
        public override DepthInfo CurrentDepthInfo => GLWrapper.CurrentDepthInfo;
        public override bool IsMaskingActive => GLWrapper.IsMaskingActive;

        public override void PushViewport(RectangleI viewport) => GLWrapper.PushViewport(viewport);

        public override void PopViewport() => GLWrapper.PopViewport();

        public override void PushOrtho(RectangleF ortho) => GLWrapper.PushOrtho(ortho);

        public override void PopOrtho() => GLWrapper.PopOrtho();

        public override void PushMaskingInfo(MaskingInfo maskingInfo, bool overwritePreviousScissor = false) => GLWrapper.PushMaskingInfo(maskingInfo, overwritePreviousScissor);

        public override void PopMaskingInfo() => GLWrapper.PopMaskingInfo();

        public override void PushDepthInfo(DepthInfo depthInfo) => GLWrapper.PushDepthInfo(depthInfo);

        public override void PopDepthInfo() => GLWrapper.PopDepthInfo();

        public override void Clear(ClearInfo clearInfo) => GLWrapper.Clear(clearInfo);

        public override void PushScissorState(bool enabled) => GLWrapper.PushScissorState(enabled);

        public override void PopScissorState() => GLWrapper.PopScissorState();
    }
}
