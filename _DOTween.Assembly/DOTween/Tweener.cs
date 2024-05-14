// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/12 16:24
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php


using System;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using UnityEngine;
using UnityEngine.Assertions;

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

        internal Tweener() {}

        // ===================================================================================
        // ABSTRACT METHODS ------------------------------------------------------------------

        internal abstract Tweener SetFrom(bool relative);

        public abstract void ApplyOriginal();

        // ===================================================================================
        // INTERNAL METHODS ------------------------------------------------------------------

        // CALLED BY DOTween when spawning/creating a new Tweener.
        // Returns TRUE if the setup is successful
        internal static bool Setup<T1, T2>(
            TweenerCore<T1, T2> t, DOGetter<T1> getter, DOSetter<T1> setter, T2 endValue, float duration, ABSTweenPlugin<T1, T2> plugin
        )
        {
            Assert.IsNotNull(plugin, "Given plugin is null");

            t.tweenPlugin = plugin;
            t.getter = getter;
            t.setter = setter;
            t.endValue = endValue;
            t.duration = duration;
            // Defaults
            t.autoKill = true;
            t.isRecyclable = true;
            t.easeType = DOTween.defaultEaseType; // Set to INTERNAL_Zero in case of 0 duration, but in DoStartup
            t.easeOvershootOrAmplitude = DOTween.defaultEaseOvershootOrAmplitude;
            t.easePeriod = DOTween.defaultEasePeriod;
            t.loopType = LoopType.Restart;
            t.isPlaying = true;
            return true;
        }

        // CALLED BY TweenerCore
        // Returns the elapsed time minus delay in case of success,
        // -1 if there are missing references and the tween needs to be killed
        internal static float DoUpdateDelay<T1, T2>(TweenerCore<T1, T2> t, float elapsed)
        {
            float tweenDelay = t.delay;
            if (elapsed > tweenDelay) {
                // Delay complete
                t.elapsedDelay = tweenDelay;
                t.delayComplete = true;
                return elapsed - tweenDelay;
            }
            t.elapsedDelay = elapsed;
            return 0;
        }
    }
}