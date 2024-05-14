// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 00:41
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;

#pragma warning disable 1591
namespace DG.Tweening.Plugins.Core
{
    // Public so it can be extended by custom plugins
    public abstract class TweenPlugin<T> : ITweenPlugin
    {
        public abstract void SetFrom(TweenerCore<T> t, bool isRelative);
        public abstract void SetFrom(TweenerCore<T> t, T fromValue, bool setImmediately, bool isRelative);
        public abstract void SetRelativeEndValue(TweenerCore<T> t);
        public abstract void SetChangeValue(TweenerCore<T> t);
        public abstract void EvaluateAndApply(TweenerCore<T> t, float elapsed);
    }
}