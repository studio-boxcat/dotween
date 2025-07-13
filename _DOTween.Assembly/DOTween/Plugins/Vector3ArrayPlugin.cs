// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/20 15:05
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

// ReSharper disable InconsistentNaming

using System;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Plugins.Core;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Plugins
{
    /// <summary>
    /// This plugin generates some GC allocations at startup
    /// </summary>
    public class Vector3ArrayPlugin : TweenPlugin<Vector3>
    {
        public static readonly Vector3ArrayPlugin Instance = new();

        public override void SetFrom(TweenerCore<Vector3> t, bool isRelative)
            => throw new NotSupportedException("Vector3ArrayPlugin does not support the SetFrom method");
        public override void SetFrom(TweenerCore<Vector3> t, Vector3 fromValue, bool setImmediately, bool isRelative)
            => throw new NotSupportedException("Vector3ArrayPlugin does not support the SetFrom method");
        public override void SetRelativeEndValue(TweenerCore<Vector3> t)
            => throw new NotSupportedException("Vector3ArrayPlugin does not support the relative endValue");

        // For Punch & Shake, the endValue must be same as startValue.
        public override void SetChangeValue(TweenerCore<Vector3> t) => t.changeValue = default;

        public override void EvaluateAndApply(TweenerCore<Vector3> t, float elapsed)
        {
            Assert.IsFalse(t.isRelative, "Vector3ArrayPlugin does not support relative values");
            Assert.IsFalse(t.isFrom, "Vector3ArrayPlugin does not support From values");

#if DEBUG
            if (t.easeType is not (Ease.Linear or Ease.OutQuad))
                L.W("Vector3ArrayPlugin only supports Linear and OutQuad ease types", t);
#endif

            var duration = t.duration;
            var opts = (Vector3ArrayOptions) t.plugOptions;
            opts!.Resolve(elapsed / duration, out var segmentTime, out var segmentDuration, out var segmentStartValue, out var segmentChangeValue);

            var value = t.startValue + segmentStartValue;
            if (segmentDuration is not 0)
            {
                var easeVal = EaseManager.Evaluate(t.easeType, t.customEase, segmentTime, segmentDuration, t.easeOvershootOrAmplitude, t.easePeriod);
                value += segmentChangeValue * easeVal;
            }

            t.setter(value);
        }
    }

    public class Vector3ArrayOptions
    {
        private readonly float[] startTimes; // normalized time values (0-1)
        private readonly Vector3[] startValues;

        public Vector3ArrayOptions(float[] startTimes, Vector3[] startValues)
        {
            Assert.AreEqual(startTimes.Length, startValues.Length, "startTimes and startValues must have the same length");
            Assert.IsTrue(startTimes[0] > 0, "First time value must be greater than 0");
            Assert.IsFalse(startTimes[^1] is 0 or 1, "Last time value must be less than 1");
            Assert.IsTrue(startValues[0] != Vector3.zero, "Last value must be different from Vector3.zero");
            Assert.IsTrue(startValues[^1] != Vector3.zero, "Last value must be different from Vector3.zero");

            this.startTimes = startTimes;
            this.startValues = startValues;
        }

        public void Resolve(float t, out float segmentTime, out float segmentDuration, out Vector3 segmentStartValue, out Vector3 segmentChangeValue)
        {
            if (t >= 1)
            {
                segmentTime = 0;
                segmentDuration = 0;
                segmentStartValue = default;
                segmentChangeValue = default;
                return;
            }

            if (t < startTimes[0])
            {
                segmentTime = t;
                segmentDuration = startTimes[0];
                segmentStartValue = default;
                segmentChangeValue = startValues[0];
                return;
            }

            for (var i = 0; i < startTimes.Length - 1; i++)
            {
                var curEndTime = startTimes[i + 1];
                if (curEndTime < t) continue;

                var curStartTime = startTimes[i];
                segmentTime = t - curStartTime;
                segmentDuration = curEndTime - curStartTime;

                segmentStartValue = startValues[i];
                var curEndValue = startValues[i + 1];
                segmentChangeValue = curEndValue - segmentStartValue;
                return;
            }

            segmentTime = t - startTimes[^1];
            segmentDuration = 1 - segmentTime;
            segmentStartValue = startValues[^1];
            segmentChangeValue = -segmentStartValue;
        }
    }
}