﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.Drawing;
using System.IO;
using OpenTK.Graphics.ES30;
using osu.Framework.Graphics.OpenGL.Textures;

namespace osu.Framework.Graphics.Textures
{
    public static class TextureLoader
    {
        /// <summary>
        /// Creates a texture from bytes laid out in BGRA format, row major.
        /// </summary>
        /// <param name="data">The raw bytes containing the texture in provided format, row major.</param>
        /// <param name="width">Width of the texture in pixels.</param>
        /// <param name="height">Height of the texture in pixels.</param>
        /// <param name="atlas">The atlas to add the texture to.</param>
        /// <param name="format">The pixel format of the data.</param>
        /// <returns>The created texture.</returns>
        public static Texture FromRawBytes(byte[] data, int width, int height, TextureAtlas atlas = null, PixelFormat format = PixelFormat.Rgba)
        {
            if (data == null)
                return null;

            Texture tex = atlas == null ? new Texture(width, height) : atlas.Add(width, height);

            var upload = new TextureUpload(data)
            {
                Format = format
            };
            tex.SetData(upload);
            return tex;
        }
    }
}
