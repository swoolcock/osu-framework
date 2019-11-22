// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Platform
{
    public readonly struct DisplayMode
    {
        public readonly int Index;
        public readonly uint PixelFormat;
        public readonly int Width;
        public readonly int Height;
        public readonly string PixelFormatName;

        public DisplayMode(int index, uint pixelFormat, int width, int height, string pixelFormatName)
        {
            Index = index;
            PixelFormat = pixelFormat;
            Width = width;
            Height = height;
            PixelFormatName = pixelFormatName;
        }

        public override string ToString() => $"Mode {Index}: ({Width}x{Height}), Format: {PixelFormatName}";
    }
}
