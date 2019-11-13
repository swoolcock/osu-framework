// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Backends
{
    public class BackendMismatchException : Exception
    {
        public BackendMismatchException(Type source, Type expected)
            : base($"{source.Name} requires a corresponding {expected.Name}")
        {
        }
    }
}
