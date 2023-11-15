// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/05/06 16:33
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class FloatPlugin : ABSTweenPlugin<float,NoOptions>
    {
        public override void SetFrom(TweenerCore<float, float, NoOptions> t, bool isRelative)
        {
            float prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }
        public override void SetFrom(TweenerCore<float, float, NoOptions> t, float fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative) {
                float currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(fromValue);
        }

        public override void SetRelativeEndValue(TweenerCore<float, float, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<float, float, NoOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(
            NoOptions options, Tween t, bool isRelative, DOGetter<float> getter, DOSetter<float> setter,
            float elapsed, float startValue, float changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
            UpdateNotice updateNotice
        ){
            if (t.loopType == LoopType.Incremental) startValue += changeValue * (t.isComplete ? t.completedLoops - 1 : t.completedLoops);
            if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental) {
                startValue += changeValue * (t.loopType == LoopType.Incremental ? t.loops : 1)
                    * (t.sequenceParent.isComplete ? t.sequenceParent.completedLoops - 1 : t.sequenceParent.completedLoops);
            }

            setter(startValue + changeValue * EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod));
        }
    }
}