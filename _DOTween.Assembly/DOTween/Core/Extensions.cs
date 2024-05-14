// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/27 10:30
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

namespace DG.Tweening.Core
{
    /// <summary>
    /// Public only so custom shortcuts can access some of these methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// INTERNAL: used by DO shortcuts and Modules to set the tween as blendable
        /// </summary>
        public static TweenerCore<T> Blendable<T>(this TweenerCore<T> t)
        {
            t.isBlendable = true;
            return t;
        }

        /// <summary>
        /// INTERNAL: used by DO shortcuts and Modules to prevent a tween from using a From setup even if passed
        /// </summary>
        public static TweenerCore<T> NoFrom<T>(this TweenerCore<T> t)
        {
            t.isFromAllowed = false;
            return t;
        }
    }
}