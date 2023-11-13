// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/17 19:40
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using UnityEngine;

namespace DG.Tweening.Core
{
    /// <summary>
    /// Various utils
    /// </summary>
    public static class DOTweenUtils
    {
        /// <summary>
        /// Returns a Vector3 with z = 0
        /// </summary>
        internal static Vector3 Vector3FromAngle(float degrees, float magnitude)
        {
            float radians = degrees * Mathf.Deg2Rad;
            return new Vector3(magnitude * Mathf.Cos(radians), magnitude * Mathf.Sin(radians), 0);
        }

        /// <summary>
        /// Returns the 2D angle between two vectors
        /// </summary>
        internal static float Angle2D(Vector3 from, Vector3 to)
        {
            Vector2 baseDir = Vector2.right;
            to -= from;
            float ang = Vector2.Angle(baseDir, to);
            Vector3 cross = Vector3.Cross(baseDir, to);
            if (cross.z > 0) ang = 360 - ang;
            ang *= -1f;
            return ang;
        }

        internal static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }

        /// <summary>
        /// Returns a point on a circle with the given center and radius,
        /// using Unity's circle coordinates (0° points up and increases clockwise)
        /// </summary>
        public static Vector2 GetPointOnCircle(Vector2 center, float radius, float degrees)
        {
            // Adapt to Unity's circle coordinates
            // (Unity's circle 0° points up and increases clockwise, unit circle 0° points right and rotates counter-clockwise)
            degrees = 90 - degrees;
            //
            float radians = degrees * Mathf.Deg2Rad;
            return center + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
        }

        /// <summary>
        /// Uses approximate equality on each axis instead of Unity's Vector3 equality,
        /// because the latter fails (in some cases) when assigning a Vector3 to a transform.position and then checking it.
        /// </summary>
        internal static bool Vector3AreApproximatelyEqual(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x)
                   && Mathf.Approximately(a.y, b.y)
                   && Mathf.Approximately(a.z, b.z);
        }
    }
}