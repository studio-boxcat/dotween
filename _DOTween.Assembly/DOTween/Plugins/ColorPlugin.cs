﻿#if !COMPATIBLE
// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/10 14:33
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    public class ColorPlugin : ABSTweenPlugin<Color, NoOptions>
    {
        public override void SetFrom(TweenerCore<Color, Color, NoOptions> t, bool isRelative)
        {
            Color prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            Color to = t.endValue;
            to = t.startValue;
            t.setter(to);
        }
        public override void SetFrom(TweenerCore<Color, Color, NoOptions> t, Color fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative) {
                Color currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) {
                t.setter(fromValue);
            }
        }

        public override void SetRelativeEndValue(TweenerCore<Color, Color, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Color, Color, NoOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override void EvaluateAndApply(
            NoOptions options, Tween t, DOGetter<Color> getter, DOSetter<Color> setter,
            float elapsed, Color startValue, Color changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
            UpdateNotice updateNotice
        ){
            if (t.loopType == LoopType.Incremental) startValue += changeValue * (t.isComplete ? t.completedLoops - 1 : t.completedLoops);
            if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental) {
                startValue += changeValue * (t.loopType == LoopType.Incremental ? t.loops : 1)
                    * (t.sequenceParent.isComplete ? t.sequenceParent.completedLoops - 1 : t.sequenceParent.completedLoops);
            }

            float easeVal = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
            startValue.r += changeValue.r * easeVal;
            startValue.g += changeValue.g * easeVal;
            startValue.b += changeValue.b * easeVal;
            startValue.a += changeValue.a * easeVal;
            setter(startValue);
        }
    }
}
#endif