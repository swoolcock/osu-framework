// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

#if __IOS__
extern alias IOS;
using IOS::UIKit;
using IOS::CoreGraphics;
using IOS::Foundation;
#endif

using System.Diagnostics;
using System.IO;
using PixelFormat = OpenTK.Graphics.ES30.PixelFormat;
using System.Runtime.InteropServices;
using System;

namespace osu.Framework.Graphics.Textures
{
    public class RawTexture
    {
        public int Width, Height;
        public PixelFormat PixelFormat;
        public byte[] Pixels;

#if __IOS__
        public unsafe static RawTexture FromUIImage(UIImage image)
        {
            if (image == null)
                return null;

            int width = (int)image.Size.Width;
            int height = (int)image.Size.Height;

            IntPtr data = Marshal.AllocHGlobal(width * height * 4);

            using (CGBitmapContext textureContext = new CGBitmapContext(data, width, height, 8, width * 4, image.CGImage.ColorSpace, CGImageAlphaInfo.PremultipliedLast))
                textureContext.DrawImage(new IOS::System.Drawing.RectangleF(0, 0, width, height), image.CGImage);

            RawTexture t = new RawTexture
            {
                Width = width,
                Height = height,
                Pixels = new byte[width * height * 4],
                PixelFormat = PixelFormat.Rgba
            };

            Marshal.Copy(data, t.Pixels, 0, t.Pixels.Length);
            Marshal.FreeHGlobal(data);

            return t;
        }

        public static RawTexture FromStream(Stream stream)
        {
            return FromUIImage(UIImage.LoadFromData(NSData.FromStream(stream)));
        }
#else
        public static RawTexture FromStream(Stream stream)
        {
            using (Bitmap bitmap = new Bitmap(stream))
            {
                var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                RawTexture t = new RawTexture
                {
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    Pixels = new byte[data.Width * data.Height * 4],
                    PixelFormat = PixelFormat.Rgba
                };

                unsafe
                {
                    //convert from BGRA (System.Drawing) to RGBA
                    //don't need to consider stride because we're in a raw format
                    var src = (byte*)data.Scan0;

                    Debug.Assert(src != null);

                    fixed (byte* pixels = t.Pixels)
                    {
                        var dest = pixels;

                        int length = t.Pixels.Length / 4;
                        for (int i = 0; i < length; i++)
                        {
                            //BGRA -> RGBA
                            // ReSharper disable once PossibleNullReferenceException
                            dest[0] = src[2];
                            dest[1] = src[1];
                            dest[2] = src[0];
                            dest[3] = src[3];

                            src += 4;
                            dest += 4;
                        }
                    }
                }

                bitmap.UnlockBits(data);

                return t;
            }
        }
#endif
    }
}
