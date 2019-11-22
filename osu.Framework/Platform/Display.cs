// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Platform
{
    public class Display
    {
        public int Index;

        public DisplayMode[] DisplayModes { get; }

        public Display(int index, DisplayMode[] displayModes)
        {
            Index = index;
            DisplayModes = displayModes;
        }

        public override string ToString()
        {
            string rv = $"Display {Index}:";
            foreach (var mode in DisplayModes)
                rv += $"\n- {mode.ToString()}";
            return rv;
        }
    }
}
