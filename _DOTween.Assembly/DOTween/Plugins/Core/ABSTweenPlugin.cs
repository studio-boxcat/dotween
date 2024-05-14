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
    public abstract class ABSTweenPlugin<T1,T2> : ITweenPlugin
    {
        public virtual void Reset(TweenerCore<T1, T2> t) { } // Resets specific TweenerCore stuff, not the plugin itself
        public abstract void SetFrom(TweenerCore<T1, T2> t, bool isRelative);
        public abstract void SetFrom(TweenerCore<T1, T2> t, T2 fromValue, bool setImmediately, bool isRelative);
        public abstract T2 ConvertToStartValue(TweenerCore<T1, T2> t, T1 value);
        public abstract void SetRelativeEndValue(TweenerCore<T1, T2> t);
        public abstract void SetChangeValue(TweenerCore<T1, T2> t);
        public abstract void ApplyOriginal(TweenerCore<T1, T2> t);
        public abstract void EvaluateAndApply(TweenerCore<T1, T2> t, bool useInversePosition);
    }

    public abstract class ABSTweenPlugin<T1> : ABSTweenPlugin<T1, T1>
    {
        public override T1 ConvertToStartValue(TweenerCore<T1, T1> t, T1 value) => value;
        public override void ApplyOriginal(TweenerCore<T1, T1> t) => t.setter(t.isFrom ? t.endValue : t.startValue);
    }
}