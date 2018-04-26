﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;

namespace osu.Framework.Graphics.Effects
{
    /// <summary>
    /// Creates an outline around the drawable this effect gets applied to.
    /// </summary>
    public class OutlineEffect
#if __IOS__
        : IEffect<Container>
#else
        : IEffect<BufferedContainer>
#endif
    {
        /// <summary>
        /// The strength of the outline. A higher strength means that the blur effect used to draw the outline fades slower.
        /// Default is 1.
        /// </summary>
        public float Strength = 1f;

        /// <summary>
        /// The sigma value for the blur effect used to draw the outline. This controls over how many pixels the outline gets spread.
        /// Default is <see cref="Vector2.One"/>.
        /// </summary>
        public Vector2 BlurSigma = Vector2.One;

        /// <summary>
        /// The color of the outline. Default is <see cref="Color4.Black"/>.
        /// </summary>
        public ColourInfo Colour = Color4.Black;

        /// <summary>
        /// Whether to automatically pad by the blur extent such that no clipping occurs at the sides of the effect. Default is false.
        /// </summary>
        public bool PadExtent;

        /// <summary>
        /// True if the effect should be cached. This is an optimization, but can cause issues if the drawable changes the way it looks without changing its size.
        /// Turned off by default.
        /// </summary>
        public bool CacheDrawnEffect;

#if __IOS__
        public Container ApplyTo(Drawable drawable) =>
#else
        public BufferedContainer ApplyTo(Drawable drawable) =>
#endif
        drawable.WithEffect(new BlurEffect
        {
            Strength = Strength,
            Sigma = BlurSigma,
            Colour = Colour,
            PadExtent = PadExtent,
            CacheDrawnEffect = CacheDrawnEffect,

            DrawOriginal = true,
        });
    }
}
