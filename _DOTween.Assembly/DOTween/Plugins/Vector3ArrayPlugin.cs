// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/20 15:05
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    /// <summary>
    /// This plugin generates some GC allocations at startup
    /// </summary>
    public class Vector3ArrayPlugin : ABSTweenPlugin<Vector3, Vector3[]>
    {
        public static readonly Vector3ArrayPlugin Instance = new();

        public override void Reset(TweenerCore<Vector3, Vector3[]> t)
        {
            t.startValue = t.endValue = t.changeValue = null;
        }

        public override void SetFrom(TweenerCore<Vector3, Vector3[]> t, bool isRelative) { }
        public override void SetFrom(TweenerCore<Vector3, Vector3[]> t, Vector3[] fromValue, bool setImmediately, bool isRelative) { }

        public override Vector3[] ConvertToStartValue(TweenerCore<Vector3, Vector3[]> t, Vector3 value)
        {
            int len = t.endValue.Length;
            Vector3[] res = new Vector3[len];
            for (int i = 0; i < len; i++) {
                if (i == 0) res[i] = value;
                else res[i] = t.endValue[i - 1];
            }
            return res;
        }

        public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3[]> t)
        {
            int len = t.endValue.Length;
            for (int i = 0; i < len; ++i) {
                if (i > 0) t.startValue[i] = t.endValue[i - 1];
                t.endValue[i] = t.startValue[i] + t.endValue[i];
            }
        }

        public override void SetChangeValue(TweenerCore<Vector3, Vector3[]> t)
        {
            int len = t.endValue.Length;
            t.changeValue = new Vector3[len];
            for (int i = 0; i < len; ++i) t.changeValue[i] = t.endValue[i] - t.startValue[i];
        }

        public override void ApplyOriginal(TweenerCore<Vector3, Vector3[]> t)
        {
            Assert.IsFalse(t.isFromAllowed);
            t.setter(t.endValue[^1]);
        }

        public override void EvaluateAndApply(TweenerCore<Vector3, Vector3[]> t, bool useInversePosition){
            var startValue = t.startValue;
            var changeValue = t.changeValue;

            Vector3 incrementValue = Vector3.zero;
            if (t.loopType == LoopType.Incremental) {
                int iterations = t.isComplete ? t.completedLoops - 1 : t.completedLoops;
                if (iterations > 0) {
                    int end = startValue.Length - 1;
                    incrementValue = (startValue[end] + changeValue[end] - startValue[0]) * iterations;
                }
            }
            if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental) {
                int iterations = (t.loopType == LoopType.Incremental ? t.loops : 1)
                                 * (t.sequenceParent.isComplete ? t.sequenceParent.completedLoops - 1 : t.sequenceParent.completedLoops);
                if (iterations > 0) {
                    int end = startValue.Length - 1;
                    incrementValue += (startValue[end] + changeValue[end] - startValue[0]) * iterations;
                }
            }

            // Find correct index and segmentElapsed
            int index = 0;
            float segmentElapsed = 0;
            float segmentDuration = 0;
            var elapsed = useInversePosition ? t.duration - t.position : t.position;
            var durations = ((Vector3ArrayOptions) t.plugOptions).durations;
            int len = durations.Length;
            float count = 0;
            for (int i = 0; i < len; i++) {
                segmentDuration = durations[i];
                count += segmentDuration;
                if (elapsed > count) {
                    segmentElapsed += segmentDuration;
                    continue;
                }
                index = i;
                segmentElapsed = elapsed - segmentElapsed;
                break;
            }

            // Evaluate
            var easeVal = EaseManager.Evaluate(t.easeType, t.customEase, segmentElapsed, segmentDuration, t.easeOvershootOrAmplitude, t.easePeriod);
            t.setter(startValue[index] + incrementValue + changeValue[index] * easeVal);
        }
    }
}