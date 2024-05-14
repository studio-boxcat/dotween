// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/28 11:23
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using DG.Tweening.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace DG.Tweening.Plugins.Options
{
    public class VectorOptions
    {
        public AxisConstraint axisConstraint;

        public static bool GetAxisConstraints([CanBeNull] object opts, out bool x, out bool y)
        {
            x = y = false;
            if (opts is null) return false;

            var constraint = ((VectorOptions) opts).axisConstraint;
            if ((constraint & AxisConstraint.X) is AxisConstraint.X)
                x = true;
            if ((constraint & AxisConstraint.Y) is AxisConstraint.Y)
                y = true;
            return x || y;
        }

        public static void SetAxisConstraint(TweenerCore<Vector2> t, AxisConstraint axisConstraint)
        {
            var o = t.plugOptions;
            if (o is null)
            {
                t.plugOptions = new VectorOptions { axisConstraint = axisConstraint };
                return;
            }

            var opts = (VectorOptions) o;
            opts.axisConstraint = axisConstraint;
        }

        public static void SetAxisConstraint(TweenerCore<Vector3> t, AxisConstraint axisConstraint)
        {
            var o = t.plugOptions;
            if (o is null)
            {
                t.plugOptions = new VectorOptions { axisConstraint = axisConstraint };
                return;
            }

            var opts = (VectorOptions) o;
            opts.axisConstraint = axisConstraint;
        }
    }
}