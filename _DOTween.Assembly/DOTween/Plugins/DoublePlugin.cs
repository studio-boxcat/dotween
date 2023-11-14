﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/10/09 10:53
// License Copyright (c) Daniele Giardini
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
    public class DoublePlugin : ABSTweenPlugin<double, NoOptions>
    {
        public override void SetFrom(TweenerCore<double, double, NoOptions> t, bool isRelative)
        {
            double prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }
        public override void SetFrom(TweenerCore<double, double, NoOptions> t, double fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative) {
                double currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(fromValue);
        }

        public override void SetRelativeEndValue(TweenerCore<double, double, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<double, double, NoOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(
            NoOptions options, Tween t, bool isRelative, DOGetter<double> getter, DOSetter<double> setter,
            float elapsed, double startValue, double changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
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