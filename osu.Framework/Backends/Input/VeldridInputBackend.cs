// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Configuration;
using osu.Framework.Input.Handlers;

namespace osu.Framework.Backends.Input
{
    public class VeldridInputBackend : InputBackend
    {
        public override IEnumerable<InputHandler> CreateInputHandlers() => new InputHandler[0];

        public override void Configure(ConfigManager<FrameworkSetting> config)
        {
        }
    }
}
