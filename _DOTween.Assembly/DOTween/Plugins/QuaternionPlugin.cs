#if !COMPATIBLE
// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/07 20:02
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
    public class QuaternionPlugin : ABSTweenPlugin<Quaternion, Vector3, NoOptions>
    {
        public override void SetFrom(TweenerCore<Quaternion, Vector3, NoOptions> t, bool isRelative)
        {
            var prevEndVal = t.endValue;
            t.endValue = t.getter().eulerAngles;
            t.startValue = GetEulerValForCalculations(t, t.endValue + prevEndVal, t.endValue);
            t.setter(Quaternion.Euler(t.startValue));
        }

        public override void SetFrom(TweenerCore<Quaternion, Vector3, NoOptions> t, Vector3 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                var currVal = t.getter().eulerAngles;
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = GetEulerValForCalculations(t, fromValue, t.endValue);
            if (setImmediately) t.setter(Quaternion.Euler(fromValue));
        }

        public override Vector3 ConvertToStartValue(TweenerCore<Quaternion, Vector3, NoOptions> t, Quaternion value)
        {
            return value.eulerAngles;
        }

        public override void SetRelativeEndValue(TweenerCore<Quaternion, Vector3, NoOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Quaternion, Vector3, NoOptions> t)
        {
            var endVal = t.isFrom ? t.endValue : GetEulerValForCalculations(t, t.endValue, t.startValue);
            var startVal = t.startValue;
            t.changeValue = endVal - startVal;
        }

        public override void ApplyOriginal(TweenerCore<Quaternion, Vector3, NoOptions> t)
        {
            var value = t.isFrom ? t.endValue : t.startValue;
            t.setter(Quaternion.Euler(value));
        }

        public override void EvaluateAndApply(
            NoOptions options, Tween t, DOGetter<Quaternion> getter, DOSetter<Quaternion> setter,
            float elapsed, Vector3 startValue, Vector3 changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
            UpdateNotice updateNotice
        )
        {
            var endValue = startValue;

            if (t.loopType == LoopType.Incremental)
                endValue += changeValue * (t.isComplete ? t.completedLoops - 1 : t.completedLoops);

            if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
            {
                var a = t.loopType == LoopType.Incremental ? t.loops : 1;
                var b = t.sequenceParent.isComplete ? t.sequenceParent.completedLoops - 1 : t.sequenceParent.completedLoops;
                endValue += changeValue * (a * b);
            }

            var easeVal = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
            endValue.x += changeValue.x * easeVal;
            endValue.y += changeValue.y * easeVal;
            endValue.z += changeValue.z * easeVal;
            setter(Quaternion.Euler(endValue));
        }

        // This fixes wobbling when rotating on a single axis in some cases
        static Vector3 GetEulerValForCalculations(TweenerCore<Quaternion, Vector3, NoOptions> t, Vector3 val, Vector3 counterVal)
        {
            if (t.isRelative) return val;

            Vector3 valFlipped = FlipEulerAngles(val);

            bool xVsNormalSame = Mathf.Approximately(counterVal.x, val.x);
            bool xVsFlippedSame = Mathf.Approximately(counterVal.x, valFlipped.x);
            bool yVsNormalSame = Mathf.Approximately(counterVal.y, val.y);
            bool yVsFlippedSame = Mathf.Approximately(counterVal.y, valFlipped.y);
            bool zVsNormalSame = Mathf.Approximately(counterVal.z, val.z);
            bool zVsFlippedSame = Mathf.Approximately(counterVal.z, valFlipped.z);

            bool isSingleAxisRotationNormal = xVsNormalSame && (yVsNormalSame || zVsNormalSame)
                                              || yVsNormalSame && zVsNormalSame;
            bool isSingleAxisRotationFlipped = !isSingleAxisRotationNormal
                                               && xVsFlippedSame && (yVsFlippedSame || zVsFlippedSame)
                                               || yVsFlippedSame && zVsFlippedSame;

            // Debug.Log(counterVal + " - " + val + " / " + valFlipped + " ► isSingleAxisNormal: " + isSingleAxisRotationNormal + " / isSingleAxisFlipped: " + isSingleAxisRotationFlipped);
            if (!isSingleAxisRotationNormal && !isSingleAxisRotationFlipped) return val;

            // Debug.Log("► Single Axis Rotation");
            int axisToRotate = 0;
            if (isSingleAxisRotationNormal)
            {
                axisToRotate = xVsNormalSame
                    ? yVsNormalSame
                        ? 2 : 1
                    : 0;
            }
            else
            {
                axisToRotate = xVsFlippedSame
                    ? yVsFlippedSame
                        ? 2 : 1
                    : 0;
            }
            bool flip = false;
            switch (axisToRotate)
            {
                case 0: // X
                    flip = !Mathf.Approximately(counterVal.y, val.y) || !Mathf.Approximately(counterVal.z, val.z);
                    break;
                case 1: // Y
                    flip = !Mathf.Approximately(counterVal.x, val.x) || !Mathf.Approximately(counterVal.z, val.z);
                    break;
                case 2: // Z
                    flip = !Mathf.Approximately(counterVal.x, val.x) || !Mathf.Approximately(counterVal.y, val.y);
                    break;
            }
            return flip ? valFlipped : val;

            // Flips the euler angles from one representation to the other
            static Vector3 FlipEulerAngles(Vector3 euler)
            {
                return new Vector3(180 - euler.x, euler.y + 180, euler.z + 180);
            }
        }
    }
}
#endif