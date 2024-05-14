using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
    static class SpecialTweenUtils
    {
        internal static void SetupPunch(TweenerCore<Vector3, Vector3> t)
        {
            t.isRelative = false;
            t.easeType = Ease.OutQuad;
            t.customEase = null;
            t.NoFrom();
        }

        internal static void SetupShake(TweenerCore<Vector3, Vector3> t)
        {
            t.isRelative = false;
            t.easeType = Ease.Linear;
            t.customEase = null;
            t.NoFrom();
        }

        public static Vector3ArrayOptions CalculatePunch(Vector3 direction, float duration, int vibrato, float elasticity)
        {
            var segmentCount = (int) (vibrato * duration);
            if (segmentCount < 2) segmentCount = 2;

            // Calculate and store the duration of each tween
            var startTimes = new float[segmentCount - 1]; // Start time for the first segment is omitted.
            var segmentDuration = duration / segmentCount;
            for (var i = 0; i < segmentCount - 1; ++i)
                startTimes[i] = segmentDuration * (i + 1);

            // Create the tween
            var startValues = new Vector3[segmentCount]; // Start value for the first segment is omitted.
            startValues[0] = direction;
            var strength = direction.magnitude;
            direction /= strength; // Normalize the direction.
            var strengthDecay = strength / segmentCount;
            elasticity = Mathf.Clamp01(elasticity);
            for (var i = 1; i < segmentCount - 1; ++i)
            {
                var value = direction * (strength * elasticity);
                if (i % 2 != 0) value = -value; // When i is odd, invert the direction.
                startValues[i] = value;
                strength -= strengthDecay; // Decrease the strength.
            }

            return new Vector3ArrayOptions(startTimes, startValues);
        }

        public static Vector3ArrayOptions CalculateShake(float duration,
            Vector3 strength, int vibrato, float randomness, bool ignoreZAxis, bool vectorBased,
            bool fadeOut, ShakeRandomnessMode randomnessMode)
        {
            var segmentCount = (int) (vibrato * duration);
            if (segmentCount < 2) segmentCount = 2;

            // Calculate and store the duration of each tween
            var startTimes = new float[segmentCount - 1]; // Start time for the first segment is omitted.
            if (fadeOut)
            {
                var sum = duration; // Init with the last segment duration.
                for (var i = 0; i < segmentCount - 1; ++i)
                {
                    var segmentDuration = duration * ((i + 1) / (float) segmentCount);
                    sum += segmentDuration;
                    startTimes[i] = sum;
                }

                var durationMultiplier = duration / sum; // Multiplier that allows the sum of tDurations to equal the set duration
                for (var i = 0; i < segmentCount - 1; ++i)
                    startTimes[i] *= durationMultiplier;
            }
            else
            {
                var segmentDuration = duration / segmentCount;
                for (var i = 0; i < segmentCount - 1; ++i)
                    startTimes[i] = segmentDuration * (i + 1);
            }


            // Create the tween
            var startValues = new Vector3[segmentCount - 1];
            var shakeMagnitude = vectorBased ? strength.magnitude : strength.x;
            var shakeDecay = shakeMagnitude / segmentCount;
            var ang = Random.Range(0f, 360f);
            for (var i = 0; i < segmentCount - 1; ++i)
            {
                var rndQuaternion = Quaternion.identity;
                switch (randomnessMode)
                {
                    case ShakeRandomnessMode.Harmonic:
                        ang = ang - 180 + Random.Range(0, randomness);
                        if (vectorBased || !ignoreZAxis)
                            rndQuaternion = Quaternion.AngleAxis(Random.Range(0, randomness), Vector3.up);
                        break;
                    default: // Full
                        ang = ang - 180 + Random.Range(-randomness, randomness);
                        if (vectorBased || !ignoreZAxis)
                            rndQuaternion = Quaternion.AngleAxis(Random.Range(-randomness, randomness), Vector3.up);
                        break;
                }

                var angVec = Vector3FromAngle(ang, shakeMagnitude);
                if (vectorBased)
                {
                    var to = rndQuaternion * angVec;
                    to.x = Vector3.ClampMagnitude(to, strength.x).x;
                    to.y = Vector3.ClampMagnitude(to, strength.y).y;
                    to.z = Vector3.ClampMagnitude(to, strength.z).z;
                    to = to.normalized * shakeMagnitude; // Make sure first shake uses max magnitude
                    startValues[i] = to;
                    if (fadeOut) shakeMagnitude -= shakeDecay;
                    strength = Vector3.ClampMagnitude(strength, shakeMagnitude);
                }
                else
                {
                    startValues[i] = ignoreZAxis ? angVec : rndQuaternion * angVec;
                    if (fadeOut) shakeMagnitude -= shakeDecay;
                }
            }

            return new Vector3ArrayOptions(startTimes, startValues);

            static Vector3 Vector3FromAngle(float degrees, float magnitude)
            {
                var radians = degrees * Mathf.Deg2Rad;
                return new Vector3(magnitude * Mathf.Cos(radians), magnitude * Mathf.Sin(radians), 0);
            }
        }
    }
}