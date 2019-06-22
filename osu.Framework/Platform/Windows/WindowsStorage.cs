﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace osu.Framework.Platform.Windows
{
    public class WindowsStorage : DesktopStorage
    {
        public WindowsStorage(string baseName, IGameHost host)
            : base(baseName, host)
        {
            // allows traversal of long directory/filenames beyond the standard limitations (see https://stackoverflow.com/a/5188559)
            BasePath = Regex.Replace(BasePath, @"^([a-zA-Z]):\\", @"\\?\$1:\");
        }

        public WindowsStorage(string baseName, DesktopGameHost host)
            : base(baseName, host)
        {
        }

        public override void OpenInNativeExplorer() => Process.Start("explorer.exe", GetFullPath(string.Empty));

        protected override string LocateBasePath() => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }
}
