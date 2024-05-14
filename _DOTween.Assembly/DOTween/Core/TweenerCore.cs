﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 12:56
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    // Public so it can be used with SetOptions to show the correct overload
    // and also to allow custom plugins to change start/end/changeValue.
    // T: type of value to tween
    // T: format in which value is stored while tweening
    // TPlugOptions: options type
    public class TweenerCore<T> : Tweener
    {
        // SETUP DATA ////////////////////////////////////////////////

        public T startValue, endValue, changeValue;
        public object plugOptions;
        public DOGetter<T> getter;
        public DOSetter<T> setter;
        internal TweenPlugin<T> tweenPlugin;

        #region Constructor

        internal TweenerCore()
        {
            tweenType = TweenType.Tweener;
            Reset();
        }

        public void Setup(DOGetter<T> getter, DOSetter<T> setter, T endValue, float duration, TweenPlugin<T> plugin)
        {
            this.getter = getter;
            this.setter = setter;
            this.endValue = endValue;
            this.duration = duration;
            tweenPlugin = plugin;

            // Defaults
            autoKill = true;
            isRecyclable = true;
            easeType = DOTween.defaultEaseType;
            easeOvershootOrAmplitude = DOTween.defaultEaseOvershootOrAmplitude;
            easePeriod = DOTween.defaultEasePeriod;
            loopType = LoopType.Restart;
            isPlaying = true;
        }

        // _tweenPlugin is not reset since it's useful to keep it as a reference
        internal sealed override void Reset()
        {
            base.Reset();

            getter = null;
            setter = null;
            tweenPlugin = null;
            plugOptions = null;
            hasManuallySetStartValue = false;
            isFromAllowed = true;
        }

        #endregion

        // Sets From tweens, immediately sending the target to its endValue and assigning new start/endValues.
        // Called by TweenSettings.From.
        // Plugins that don't support From:
        // - Vector3ArrayPlugin
        // - Pro > PathPlugin, SpiralPlugin
        internal override Tweener SetFrom(bool relative)
        {
            tweenPlugin.SetFrom(this, relative);
            hasManuallySetStartValue = true;
            return this;
        }
        // Sets From tweens in an alternate way where you can set the start value directly
        // (instead of setting it from the endValue).
        // Plugins that don't support From:
        // - Vector3ArrayPlugin
        // - Pro > PathPlugin, SpiralPlugin
        internal Tweener SetFrom(T fromValue, bool setImmediately, bool relative)
        {
            tweenPlugin.SetFrom(this, fromValue, setImmediately, relative);
            hasManuallySetStartValue = true;
            return this;
        }

        // CALLED BY TweenManager at each update.
        // Returns TRUE if the tween needs to be killed
        internal override float UpdateDelay(float elapsed)
        {
            return DoUpdateDelay(this, elapsed);
        }

        // CALLED BY Tween the moment the tween starts, AFTER any delay has elapsed
        // (unless it's a FROM tween, in which case it will be called BEFORE any eventual delay).
        // Returns TRUE in case of success,
        // FALSE if there are missing references and the tween needs to be killed
        internal override bool Startup()
        {
            startupDone = true;

            if (!hasManuallySetStartValue) {
                // Take start value from current target value
                if (DOTween.useSafeMode) {
                    try {
                        if (isFrom) {
                            // From tween without forced From value and where setImmediately was FALSE
                            // (contrary to other forms of From tweens its values will be set at startup)
                            SetFrom(isRelative && !isBlendable);
                            isRelative = false;
                        } else startValue = getter();
                    } catch (Exception e) {
                        Debugger.LogSafeModeCapturedError(e, this);
                        return false; // Target/field doesn't exist: kill tween
                    }
                } else {
                    if (isFrom) {
                        // From tween without forced From value and where setImmediately was FALSE
                        // (contrary to other forms of From tweens its values will be set at startup)
                        SetFrom(isRelative && !isBlendable);
                        isRelative = false;
                    }
                    else startValue = getter();
                }
            }

            if (isRelative) tweenPlugin.SetRelativeEndValue(this);

            tweenPlugin.SetChangeValue(this);

            // Duration based startup operations
            fullDuration = loops > -1 ? duration * loops : Mathf.Infinity;

            // Applied here so that the eventual duration derived from a speedBased tween has been set
            if (duration <= 0) easeType = Ease.INTERNAL_Zero;

            return true;
        }

        public override void ApplyOriginal()
        {
            tweenPlugin.ApplyOriginal(this);
        }

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed
        internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode)
        {
            if (DOTween.useSafeMode) {
                try {
                    tweenPlugin.EvaluateAndApply(this, useInversePosition);
                } catch (Exception e) {
                    // Target/field doesn't exist anymore: kill tween
                    Debugger.LogSafeModeCapturedError(e, this);
                    return true;
                }
            } else {
                tweenPlugin.EvaluateAndApply(this, useInversePosition);
            }
            return false;
        }
    }
}