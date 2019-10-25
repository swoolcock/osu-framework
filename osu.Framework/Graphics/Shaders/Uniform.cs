// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.OpenGL;

namespace osu.Framework.Graphics.Shaders
{
    public abstract class Uniform<T> : IUniformWithValue<T>
        where T : struct
    {
        public IShader Owner { get; }
        public string Name { get; }

        public bool HasChanged { get; protected set; } = true;

        public T Value;

        protected Uniform(IShader owner, string name)
        {
            Owner = owner;
            Name = name;
        }

        public void UpdateValue(ref T newValue)
        {
            if (newValue.Equals(Value))
                return;

            Value = newValue;
            HasChanged = true;

            if (CanUpdate())
                Update();
        }

        public void Update()
        {
            if (!HasChanged) return;

            UpdateUniform();
            HasChanged = false;
        }

        protected abstract void UpdateUniform();

        protected abstract bool CanUpdate();

        ref T IUniformWithValue<T>.GetValueByRef() => ref Value;
        T IUniformWithValue<T>.GetValue() => Value;
    }
}
