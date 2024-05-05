// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/09/01 18:50
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#pragma warning disable 1591
namespace DG.Tweening.Plugins.Options
{
    public struct QuaternionOptions : IPlugOptions
    {
        public RotateMode rotateMode; // Accessed by shortcuts and Modules

        public void Reset()
        {
            rotateMode = RotateMode.Fast;
        }
    }
}