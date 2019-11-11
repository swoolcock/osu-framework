// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Input.Handlers;
using Veldrid;

namespace osu.Framework.Backends.Input
{
    public class VeldridInputBackend : InputBackend
    {
        internal InputSnapshot Snapshot;

        public override IEnumerable<InputHandler> CreateInputHandlers() => new InputHandler[0];
    }
}
