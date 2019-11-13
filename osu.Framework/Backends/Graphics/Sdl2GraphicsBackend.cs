// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using osu.Framework.Backends.Graphics.OsuTK;
using osu.Framework.Backends.Window.Sdl2;
using osu.Framework.Platform;
using osuTK.Graphics;
using osuTK.Graphics.ES30;
using Veldrid.Sdl2;

namespace osu.Framework.Backends.Graphics
{
    public class Sdl2GraphicsBackend : OpenGLGraphicsBackend
    {
        internal IntPtr SdlWindowHandle;
        internal IntPtr Context;

        protected override void SetVSync(bool vsync) => Sdl2Native.SDL_GL_SetSwapInterval(vsync ? 1 : 0);

        protected override void CreateContext(IGameHost host)
        {
            if (!(host.Window is Sdl2WindowBackend window))
                throw new BackendMismatchException(GetType(), typeof(Sdl2WindowBackend));

            SdlWindowHandle = window.SdlWindow;
            Context = Sdl2Native.SDL_GL_CreateContext(SdlWindowHandle);

            MakeCurrent();

            loadTKBindings();
        }

        public override IRenderer CreateRenderer() => new OsuTKRenderer(this);

        public override void MakeCurrent() => Sdl2Native.SDL_GL_MakeCurrent(SdlWindowHandle, Context);

        public override void SwapBuffers() => Sdl2Native.SDL_GL_SwapWindow(SdlWindowHandle);

        private void loadTKBindings()
        {
            // loadEntryPoints(new osuTK.Graphics.OpenGL.GL());
            // loadEntryPoints(new osuTK.Graphics.OpenGL4.GL());
            // loadEntryPoints(new osuTK.Graphics.ES11.GL());
            // loadEntryPoints(new osuTK.Graphics.ES20.GL());
            loadEntryPoints(new GL());
        }

        private unsafe void loadEntryPoints(GraphicsBindingsBase bindings)
        {
            var type = bindings.GetType();
            var pointsInfo = type.GetRuntimeFields().First(x => x.Name == "_EntryPointsInstance");
            var namesInfo = type.GetRuntimeFields().First(x => x.Name == "_EntryPointNamesInstance");
            var offsetsInfo = type.GetRuntimeFields().First(x => x.Name == "_EntryPointNameOffsetsInstance");

            var entryPointsInstance = (IntPtr[])pointsInfo.GetValue(bindings);
            var entryPointNamesInstance = (byte[])namesInfo.GetValue(bindings);
            var entryPointNameOffsetsInstance = (int[])offsetsInfo.GetValue(bindings);

            fixed (byte* name = entryPointNamesInstance)
            {
                for (int i = 0; i < entryPointsInstance.Length; i++)
                {
                    var ptr = name + entryPointNameOffsetsInstance[i];
                    var str = Marshal.PtrToStringAnsi(new IntPtr(ptr));
                    entryPointsInstance[i] = Sdl2Native.SDL_GL_GetProcAddress(str);
                }
            }

            pointsInfo.SetValue(bindings, entryPointsInstance);
        }
    }
}
