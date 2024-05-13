// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/07 12:56
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    // Public so it can be used with SetOptions to show the correct overload
    // and also to allow custom plugins to change start/end/changeValue.
    // T1: type of value to tween
    // T2: format in which value is stored while tweening
    // TPlugOptions: options type
    public class TweenerCore<T1,T2,TPlugOptions> : Tweener where TPlugOptions : struct
    {
        // SETUP DATA ////////////////////////////////////////////////

        public T2 startValue, endValue, changeValue;
        public TPlugOptions plugOptions;
        public DOGetter<T1> getter;
        public DOSetter<T1> setter;
        internal ABSTweenPlugin<T1, T2, TPlugOptions> tweenPlugin;

        #region Constructor

        internal TweenerCore()
        {
            typeofT1 = typeof(T1);
            typeofT2 = typeof(T2);
            typeofTPlugOptions = typeof(TPlugOptions);
            tweenType = TweenType.Tweener;
            Reset();
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
        internal Tweener SetFrom(T2 fromValue, bool setImmediately, bool relative)
        {
            tweenPlugin.SetFrom(this, fromValue, setImmediately, relative);
            hasManuallySetStartValue = true;
            return this;
        }

        // _tweenPlugin is not reset since it's useful to keep it as a reference
        internal sealed override void Reset()
        {
            base.Reset();

            tweenPlugin?.Reset(this);
            plugOptions = default; // Alternate fix that uses IPlugOptions Reset
            getter = null;
            setter = null;
            hasManuallySetStartValue = false;
            isFromAllowed = true;
        }

        // Called by TweenManager.Validate.
        // Returns TRUE if the tween is valid
        internal override bool Validate()
        {
            try {
                getter();
            } catch {
                return false;
            }
            return true;
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
            return DoStartup(this);
        }

        public override void ApplyOriginal()
        {
            tweenPlugin.ApplyOriginal(this);
        }

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed
        internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode)
        {
            var elapsed = useInversePosition ? duration - position : position;
            if (DOTween.useSafeMode) {
                try {
                    tweenPlugin.EvaluateAndApply(plugOptions, this, getter, setter, elapsed, startValue, changeValue, duration);
                } catch (Exception e) {
                    // Target/field doesn't exist anymore: kill tween
                    Debugger.LogSafeModeCapturedError($"Target or field is missing/null ({e.TargetSite}) ► {e.Message}\n\n{e.StackTrace}\n\n", this);
                    return true;
                }
            } else {
                tweenPlugin.EvaluateAndApply(plugOptions, this, getter, setter, elapsed, startValue, changeValue, duration);
            }
            return false;
        }
    }
}