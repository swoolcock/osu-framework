// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.OpenGL;

namespace osu.Framework.Graphics.Shaders
{
    internal abstract class GlobalUniform<T> : IUniformWithValue<T>
        where T : struct
    {
        public IShader Owner { get; }
        public string Name { get; }

        /// <summary>
        /// Non-null denotes a pending global change. Must be a field to allow for reference access.
        /// </summary>
        public UniformMapping<T> PendingChange;

        protected GlobalUniform(IShader owner, string name)
        {
            Owner = owner;
            Name = name;
        }

        internal void UpdateValue(UniformMapping<T> global)
        {
            PendingChange = global;
            if (CanUpdate())
                Update();
        }

        public void Update()
        {
            if (PendingChange == null)
                return;

            UpdateUniform();
            PendingChange = null;
        }

        protected abstract void UpdateUniform();
        protected abstract bool CanUpdate();
        ref T IUniformWithValue<T>.GetValueByRef() => ref PendingChange.Value;
        T IUniformWithValue<T>.GetValue() => PendingChange.Value;
    }
}
