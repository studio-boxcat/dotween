// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/08/21 12:11
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#pragma warning disable 1591
namespace DG.Tweening.Plugins.Options
{
    public class Vector3ArrayOptions
    {
        public readonly float[] durations; // Duration of each segment

        public Vector3ArrayOptions(float[] durations)
        {
            this.durations = durations;
        }
    }
}