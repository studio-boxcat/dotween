// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/20 15:05
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System;
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
            opts.Resolve(elapsed / duration, out var segmentTime, out var segmentDuration, out var segmentStartValue, out var segmentChangeValue);

            var value = t.startValue + segmentStartValue;
            if (segmentDuration is not 0)
            {
                var easeVal = EaseManager.Evaluate(t.easeType, t.customEase, segmentTime, segmentDuration, t.easeOvershootOrAmplitude, t.easePeriod);
                value += segmentChangeValue * easeVal;
            }

            t.setter(value);
        }
    }
}