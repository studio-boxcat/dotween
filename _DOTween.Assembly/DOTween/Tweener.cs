// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/12 16:24
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php


namespace DG.Tweening
{
    /// <summary>
    /// Animates a single value
    /// </summary>
    public abstract class Tweener : Tween
    {
        // TRUE when start value has been changed via From or ChangeStart/Values (allows DoStartup to take it into account).
        // Reset by TweenerCore
        internal bool hasManuallySetStartValue;
        internal bool isFromAllowed = true; // if FALSE from tweens won't be allowed. Reset by TweenerCore

        internal override void Reset()
        {
            base.Reset();
            hasManuallySetStartValue = false;
            isFromAllowed = true;
        }

        internal abstract Tweener SetFrom(bool relative);

        public abstract void ApplyOriginal();
    }
}