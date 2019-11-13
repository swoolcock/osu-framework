// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK.Input;
using Veldrid;
using Key = Veldrid.Key;
using VMouseButton = Veldrid.MouseButton;
using TKMouseButton = osuTK.Input.MouseButton;
using SNVector2 = System.Numerics.Vector2;
using TKVector2 = osuTK.Vector2;
using SDRectangle = System.Drawing.Rectangle;
using VRectangle = Veldrid.Rectangle;
using SDPoint = System.Drawing.Point;
using VPoint = Veldrid.Point;

namespace osu.Framework.Extensions
{
    public static class OsuTKExtensions
    {
        public static TKVector2 ToOsuTK(this SNVector2 vector) => new TKVector2(vector.X, vector.Y);

        public static SNVector2 ToNumerics(this TKVector2 vector) => new SNVector2(vector.X, vector.Y);

        public static TKMouseButton ToOsuTK(this VMouseButton button)
        {
            switch (button)
            {
                case VMouseButton.Left:
                    return TKMouseButton.Left;

                case VMouseButton.Middle:
                    return TKMouseButton.Middle;

                case VMouseButton.Right:
                    return TKMouseButton.Right;

                case VMouseButton.Button1:
                    return TKMouseButton.Button1;

                case VMouseButton.Button2:
                    return TKMouseButton.Button2;

                case VMouseButton.Button3:
                    return TKMouseButton.Button3;

                case VMouseButton.Button4:
                    return TKMouseButton.Button4;

                case VMouseButton.Button5:
                    return TKMouseButton.Button5;

                case VMouseButton.Button6:
                    return TKMouseButton.Button6;

                case VMouseButton.Button7:
                    return TKMouseButton.Button7;

                case VMouseButton.Button8:
                    return TKMouseButton.Button8;

                case VMouseButton.Button9:
                    return TKMouseButton.Button9;

                default:
                    return TKMouseButton.LastButton;
            }
        }

        public static VMouseButton ToVeldrid(this TKMouseButton button)
        {
            switch (button)
            {
                case TKMouseButton.Left:
                    return VMouseButton.Left;

                case TKMouseButton.Middle:
                    return VMouseButton.Middle;

                case TKMouseButton.Right:
                    return VMouseButton.Right;

                case TKMouseButton.Button1:
                    return VMouseButton.Button1;

                case TKMouseButton.Button2:
                    return VMouseButton.Button2;

                case TKMouseButton.Button3:
                    return VMouseButton.Button3;

                case TKMouseButton.Button4:
                    return VMouseButton.Button4;

                case TKMouseButton.Button5:
                    return VMouseButton.Button5;

                case TKMouseButton.Button6:
                    return VMouseButton.Button6;

                case TKMouseButton.Button7:
                    return VMouseButton.Button7;

                case TKMouseButton.Button8:
                    return VMouseButton.Button8;

                case TKMouseButton.Button9:
                    return VMouseButton.Button9;

                default:
                    return VMouseButton.LastButton;
            }
        }

        public static SDRectangle ToSystemDrawing(this VRectangle rect) =>
            new SDRectangle(rect.X, rect.Y, rect.Width, rect.Height);

        public static VRectangle ToVeldrid(this SDRectangle rect) =>
            new VRectangle(rect.X, rect.Y, rect.Width, rect.Height);

        public static SDPoint ToSystemDrawing(this VPoint point) =>
            new SDPoint(point.X, point.Y);

        public static VPoint ToVeldrid(this SDPoint point) =>
            new VPoint(point.X, point.Y);

        public static KeyEvent ToVeldrid(this KeyboardKeyEventArgs args, bool down = true)
        {
            ModifierKeys modifiers = (args.Control ? ModifierKeys.Control : 0) |
                                     (args.Shift ? ModifierKeys.Shift : 0) |
                                     (args.Alt ? ModifierKeys.Alt : 0);
            return new KeyEvent((Key)args.Key, down, modifiers);
        }
    }
}
