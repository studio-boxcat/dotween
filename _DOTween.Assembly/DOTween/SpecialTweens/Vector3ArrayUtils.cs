using DG.Tweening.Core;
using UnityEngine;

namespace DG.Tweening
{
    public static class Vector3ArrayUtils
    {
        public static (float[] Durations, Vector3[] Values) CalculatePunch(Vector3 direction, float duration, int vibrato, float elasticity)
        {
            if (elasticity > 1) elasticity = 1;
            else if (elasticity < 0) elasticity = 0;
            float strength = direction.magnitude;
            int totIterations = (int) (vibrato * duration);
            if (totIterations < 2) totIterations = 2;
            float decayXTween = strength / totIterations;
            // Calculate and store the duration of each tween
            float[] tDurations = new float[totIterations];
            float sum = 0;
            for (int i = 0; i < totIterations; ++i)
            {
                float iterationPerc = (i + 1) / (float) totIterations;
                float tDuration = duration * iterationPerc;
                sum += tDuration;
                tDurations[i] = tDuration;
            }
            float tDurationMultiplier = duration / sum; // Multiplier that allows the sum of tDurations to equal the set duration
            for (int i = 0; i < totIterations; ++i) tDurations[i] *= tDurationMultiplier;
            // Create the tween
            Vector3[] tos = new Vector3[totIterations];
            for (int i = 0; i < totIterations; ++i)
            {
                if (i < totIterations - 1)
                {
                    if (i == 0) tos[i] = direction;
                    else if (i % 2 != 0) tos[i] = -Vector3.ClampMagnitude(direction, strength * elasticity);
                    else tos[i] = Vector3.ClampMagnitude(direction, strength);
                    strength -= decayXTween;
                }
                else tos[i] = Vector3.zero;
            }

            return (tDurations, tos);
        }

        public static (float[] Durations, Vector3[] Values) CalculateShake(float duration,
            Vector3 strength, int vibrato, float randomness, bool ignoreZAxis, bool vectorBased,
            bool fadeOut, ShakeRandomnessMode randomnessMode)
        {
            float shakeMagnitude = vectorBased ? strength.magnitude : strength.x;
            int totIterations = (int) (vibrato * duration);
            if (totIterations < 2) totIterations = 2;
            float decayXTween = shakeMagnitude / totIterations;
            // Calculate and store the duration of each tween
            float[] tDurations = new float[totIterations];
            float sum = 0;
            for (int i = 0; i < totIterations; ++i)
            {
                float iterationPerc = (i + 1) / (float) totIterations;
                float tDuration = fadeOut ? duration * iterationPerc : duration / totIterations;
                sum += tDuration;
                tDurations[i] = tDuration;
            }
            float tDurationMultiplier = duration / sum; // Multiplier that allows the sum of tDurations to equal the set duration
            for (int i = 0; i < totIterations; ++i) tDurations[i] = tDurations[i] * tDurationMultiplier;
            // Create the tween
            float ang = Random.Range(0f, 360f);
            Vector3[] tos = new Vector3[totIterations];
            for (int i = 0; i < totIterations; ++i)
            {
                if (i < totIterations - 1)
                {
                    Quaternion rndQuaternion = Quaternion.identity;
                    switch (randomnessMode)
                    {
                        case ShakeRandomnessMode.Harmonic:
                            if (i > 0) ang = ang - 180 + Random.Range(0, randomness);
                            if (vectorBased || !ignoreZAxis)
                            {
                                rndQuaternion = Quaternion.AngleAxis(Random.Range(0, randomness), Vector3.up);
                            }
                            break;
                        default: // Full
                            if (i > 0) ang = ang - 180 + Random.Range(-randomness, randomness);
                            if (vectorBased || !ignoreZAxis)
                            {
                                rndQuaternion = Quaternion.AngleAxis(Random.Range(-randomness, randomness), Vector3.up);
                            }
                            break;
                    }
                    if (vectorBased)
                    {
                        Vector3 to = rndQuaternion * DOTweenUtils.Vector3FromAngle(ang, shakeMagnitude);
                        to.x = Vector3.ClampMagnitude(to, strength.x).x;
                        to.y = Vector3.ClampMagnitude(to, strength.y).y;
                        to.z = Vector3.ClampMagnitude(to, strength.z).z;
                        to = to.normalized * shakeMagnitude; // Make sure first shake uses max magnitude
                        tos[i] = to;
                        if (fadeOut) shakeMagnitude -= decayXTween;
                        strength = Vector3.ClampMagnitude(strength, shakeMagnitude);
                    }
                    else
                    {
                        if (ignoreZAxis)
                        {
                            tos[i] = DOTweenUtils.Vector3FromAngle(ang, shakeMagnitude);
                        }
                        else
                        {
                            tos[i] = rndQuaternion * DOTweenUtils.Vector3FromAngle(ang, shakeMagnitude);
                        }
                        if (fadeOut) shakeMagnitude -= decayXTween;
                    }
                }
                else tos[i] = Vector3.zero;
            }

            return (tDurations, tos);
        }
    }
}