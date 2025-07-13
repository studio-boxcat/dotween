// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/16 11:38
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#nullable enable

namespace DG.Tweening.Core
{
    internal class SequenceCallback : ABSSequentiable
    {
        public SequenceCallback(float sequencedPosition, TweenCallback callback)
        {
            this.sequencedPosition = sequencedPosition;
            onStart = callback;
        }

        public void InvokeOnStart(Sequence sequence)
        {
            onStart!.OnTweenCallback(sequence); // onStart is guaranteed to be not null here
        }
    }
}