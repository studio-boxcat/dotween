// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/21 12:11
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Plugins.Options
{
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