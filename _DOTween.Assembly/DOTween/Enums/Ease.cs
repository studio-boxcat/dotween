// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/06/30 19:20
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#pragma warning disable 1591
namespace DG.Tweening
{
    public enum Ease
    {
        Linear = 1,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack = 28,
        InBounce = 29,
        OutBounce = 30,
        InOutBounce = 31,
        /// <summary>
        /// Don't assign this! It's assigned automatically when setting the ease to an AnimationCurve or to a custom ease function
        /// </summary>
        INTERNAL_Custom = 37,
    }
}