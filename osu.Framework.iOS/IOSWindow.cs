// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace osu.Framework.iOS
{
    public class IOSWindow : Window
    {
        public IOSWindow()
            : base(new IOSWindowBackend(), new IOSGraphicsBackend())
        {
        }

        public override void SetupWindow(FrameworkConfigManager config)
        {
            base.SetupWindow(config);

            Size.Value = PrimaryDisplay.Bounds.Size;
        }
    }
}
