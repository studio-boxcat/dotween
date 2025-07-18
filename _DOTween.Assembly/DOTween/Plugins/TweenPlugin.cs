// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 00:41
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

namespace DG.Tweening
{
    public interface ITweenPlugin {}

    // Public so it can be extended by custom plugins
    public abstract class TweenPlugin<T> : ITweenPlugin where T : struct
    {
        public abstract void SetFrom(Tweener<T> t, bool isRelative);
        public abstract void SetFrom(Tweener<T> t, T fromValue, bool setImmediately, bool isRelative);
        public abstract void SetRelativeEndValue(Tweener<T> t);
        public abstract void SetChangeValue(Tweener<T> t);
        public abstract void EvaluateAndApply(Tweener<T> t, float elapsed);
    }
}