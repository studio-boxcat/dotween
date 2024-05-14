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

        // CALLED VIA Tween the moment the tween starts, AFTER any delay has elapsed
        // (unless it's a FROM tween, in which case it will be called BEFORE any eventual delay).
        // Returns TRUE in case of success,
        // FALSE if there are missing references and the tween needs to be killed
        internal static bool DoStartup<T1, T2>(TweenerCore<T1, T2> t)
        {
            t.startupDone = true;

            // Special startup operations
            if (t.specialStartupMode != SpecialStartupMode.None) {
                if (!DOStartupSpecials(t)) return false;
            }

            if (!t.hasManuallySetStartValue) {
                // Take start value from current target value
                if (DOTween.useSafeMode) {
                    try {
                        if (t.isFrom) {
                            // From tween without forced From value and where setImmediately was FALSE
                            // (contrary to other forms of From tweens its values will be set at startup)
                            t.SetFrom(t.isRelative && !t.isBlendable);
                            t.isRelative = false;
                        } else t.startValue = t.tweenPlugin.ConvertToStartValue(t, t.getter());
                    } catch (Exception e) {
                        Debugger.LogSafeModeCapturedError($"Tween startup failed (NULL target/property - {e.TargetSite}): the tween will now be killed ► {e.Message}", t);
                        return false; // Target/field doesn't exist: kill tween
                    }
                } else {
                    if (t.isFrom) {
                        // From tween without forced From value and where setImmediately was FALSE
                        // (contrary to other forms of From tweens its values will be set at startup)
                        t.SetFrom(t.isRelative && !t.isBlendable);
                        t.isRelative = false;
                    }
                    else t.startValue = t.tweenPlugin.ConvertToStartValue(t, t.getter());
                }
            }

            if (t.isRelative) t.tweenPlugin.SetRelativeEndValue(t);

            t.tweenPlugin.SetChangeValue(t);

            // Duration based startup operations
            DOStartupDurationBased(t);

            // Applied here so that the eventual duration derived from a speedBased tween has been set
            if (t.duration <= 0) t.easeType = Ease.INTERNAL_Zero;
            
            return true;
        }

        // Commands shared by DOStartup/ChangeStart/End/Values if the tween has already started up
        // and thus some settings needs to be reapplied.
        // Returns TRUE in case of SUCCESS, FALSE if there were managed errors
        static bool DOStartupSpecials<T1, T2>(TweenerCore<T1, T2> t)
        {
            try {
                switch (t.specialStartupMode) {
                case SpecialStartupMode.SetPunch:
                    if (!SpecialPluginsUtils.SetPunch(t as TweenerCore<Vector3, Vector3[]>)) return false;
                    break;
                case SpecialStartupMode.SetShake:
                    if (!SpecialPluginsUtils.SetShake(t as TweenerCore<Vector3, Vector3[]>)) return false;
                    break;
                case SpecialStartupMode.SetCameraShakePosition:
                    if (!SpecialPluginsUtils.SetCameraShakePosition(t as TweenerCore<Vector3, Vector3[]>)) return false;
                    break;
                }
                return true;
            } catch {
                // Error in SpecialPluginUtils (usually due to target being destroyed)
                return false;
            }
        }
        static void DOStartupDurationBased<T1, T2>(TweenerCore<T1, T2> t)
        {
            t.fullDuration = t.loops > -1 ? t.duration * t.loops : Mathf.Infinity;
        }
    }
}