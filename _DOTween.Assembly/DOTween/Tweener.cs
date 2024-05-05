﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/12 16:24
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php


using System;
using DOVector3 = UnityEngine.Vector3;
using DOQuaternion = UnityEngine.Quaternion;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
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
        internal static bool Setup<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, DOGetter<T1> getter, DOSetter<T1> setter, T2 endValue, float duration, ABSTweenPlugin<T1, T2, TPlugOptions> plugin = null
        )
            where TPlugOptions : struct, IPlugOptions
        {
            if (plugin != null) t.tweenPlugin = plugin;
            else {
                t.tweenPlugin ??= PluginsManager.GetDefaultPlugin<T1, T2, TPlugOptions>();
                Assert.IsNotNull(t.tweenPlugin, $"No suitable plugin found for this type: {typeof(T1)}, {typeof(T2)}, {typeof(TPlugOptions)}");
            }

            t.getter = getter;
            t.setter = setter;
            t.endValue = endValue;
            t.duration = duration;
            // Defaults
            t.autoKill = true;
            t.isRecyclable = DOTween.defaultRecyclable;
            t.easeType = DOTween.defaultEaseType; // Set to INTERNAL_Zero in case of 0 duration, but in DoStartup
            t.easeOvershootOrAmplitude = DOTween.defaultEaseOvershootOrAmplitude;
            t.easePeriod = DOTween.defaultEasePeriod;
            t.loopType = DOTween.defaultLoopType;
            t.isPlaying = true;
            return true;
        }

        // CALLED BY TweenerCore
        // Returns the elapsed time minus delay in case of success,
        // -1 if there are missing references and the tween needs to be killed
        internal static float DoUpdateDelay<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t, float elapsed) where TPlugOptions : struct, IPlugOptions
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
        internal static bool DoStartup<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct, IPlugOptions
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
                        Debugger.LogSafeModeCapturedError(string.Format(
                            "Tween startup failed (NULL target/property - {0}): the tween will now be killed ► {1}", e.TargetSite, e.Message
                        ), t);
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

        // CALLED BY TweenerCore
        internal static TweenerCore<T1, T2, TPlugOptions> DoChangeStartValue<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, T2 newStartValue, float newDuration
        ) where TPlugOptions : struct, IPlugOptions
        {
            t.hasManuallySetStartValue = true;
            t.startValue = newStartValue;

            if (t.startupDone) {
                if (t.specialStartupMode != SpecialStartupMode.None) {
                    if (!DOStartupSpecials(t)) return null;
                }
                t.tweenPlugin.SetChangeValue(t);
            }

            if (newDuration > 0) {
                t.duration = newDuration;
                if (t.startupDone) DOStartupDurationBased(t);
            }

            // Force rewind
            DoGoto(t, 0, 0, UpdateMode.IgnoreOnUpdate);

            return t;
        }

        // CALLED BY TweenerCore
        internal static TweenerCore<T1, T2, TPlugOptions> DoChangeEndValue<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, T2 newEndValue, float newDuration, bool snapStartValue
        ) where TPlugOptions : struct, IPlugOptions
        {
            t.endValue = newEndValue;
            t.isRelative = false;

            if (t.startupDone) {
                if (t.specialStartupMode != SpecialStartupMode.None) {
                    if (!DOStartupSpecials(t)) return null;
                }
                if (snapStartValue) {
                    // Reassign startValue with current target's value
                    if (DOTween.useSafeMode) {
                        try {
                            t.startValue = t.tweenPlugin.ConvertToStartValue(t, t.getter());
                        } catch (Exception e) {
                            // Target/field doesn't exist: kill tween
                            Debugger.LogSafeModeCapturedError(string.Format(
                                "Target or field is missing/null ({0}) ► {1}\n\n{2}\n\n", e.TargetSite, e.Message, e.StackTrace
                            ), t);
                            TweenManager.Despawn(t);
                            return null;
                        }
                    } else t.startValue = t.tweenPlugin.ConvertToStartValue(t, t.getter());
                }
                t.tweenPlugin.SetChangeValue(t);
            }

            if (newDuration > 0) {
                t.duration = newDuration;
                if (t.startupDone) DOStartupDurationBased(t);
            }

            // Force rewind
            DoGoto(t, 0, 0, UpdateMode.IgnoreOnUpdate);

            return t;
        }

        internal static TweenerCore<T1, T2, TPlugOptions> DoChangeValues<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, T2 newStartValue, T2 newEndValue, float newDuration
        ) where TPlugOptions : struct, IPlugOptions
        {
            t.hasManuallySetStartValue = true;
            t.isRelative = t.isFrom = false;
            t.startValue = newStartValue;
            t.endValue = newEndValue;

            if (t.startupDone) {
                if (t.specialStartupMode != SpecialStartupMode.None) {
                    if (!DOStartupSpecials(t)) return null;
                }
                t.tweenPlugin.SetChangeValue(t);
            }

            if (newDuration > 0) {
                t.duration = newDuration;
                if (t.startupDone) DOStartupDurationBased(t);
            }

            // Force rewind
            DoGoto(t, 0, 0, UpdateMode.IgnoreOnUpdate);

            return t;
        }

        // Commands shared by DOStartup/ChangeStart/End/Values if the tween has already started up
        // and thus some settings needs to be reapplied.
        // Returns TRUE in case of SUCCESS, FALSE if there were managed errors
        static bool DOStartupSpecials<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct, IPlugOptions
        {
            try {
                switch (t.specialStartupMode) {
                case SpecialStartupMode.SetLookAt:
                    if (!SpecialPluginsUtils.SetLookAt(t as TweenerCore<DOQuaternion, DOVector3, QuaternionOptions>)) return false;
                    break;
                case SpecialStartupMode.SetPunch:
                    if (!SpecialPluginsUtils.SetPunch(t as TweenerCore<Vector3, Vector3[], Vector3ArrayOptions>)) return false;
                    break;
                case SpecialStartupMode.SetShake:
                    if (!SpecialPluginsUtils.SetShake(t as TweenerCore<Vector3, Vector3[], Vector3ArrayOptions>)) return false;
                    break;
                case SpecialStartupMode.SetCameraShakePosition:
                    if (!SpecialPluginsUtils.SetCameraShakePosition(t as TweenerCore<Vector3, Vector3[], Vector3ArrayOptions>)) return false;
                    break;
                }
                return true;
            } catch {
                // Error in SpecialPluginUtils (usually due to target being destroyed)
                return false;
            }
        }
        static void DOStartupDurationBased<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct, IPlugOptions
        {
            t.fullDuration = t.loops > -1 ? t.duration * t.loops : Mathf.Infinity;
        }
    }
}