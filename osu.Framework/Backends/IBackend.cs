// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Backends
{
    /// <summary>
    /// All drivers inherit from <see cref="IDriver"/> to define them as <see cref="IDisposable"/>
    /// and provide an initialisation method that is executed after all drivers have been created.
    /// </summary>
    public interface IDriver : IDisposable
    {
        /// <summary>
        /// Performs initialisation of the driver. Drivers provided by the passed <see cref="IDriverProvider"/>
        /// will never be null, but there is no guarantee they have been <see cref="Initialise"/>d.
        /// </summary>
        /// <param name="provider">Provides uninitialised drivers.</param>
        void Initialise(IDriverProvider provider);
    }
}
