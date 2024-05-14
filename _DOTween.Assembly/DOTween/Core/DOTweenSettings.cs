// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/02/05 10:28

using System;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening.Core.Enums;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    public class DOTweenSettings : ScriptableObject
    {
        static DOTweenSettings _instanceCache = null;
        [NotNull]
        public static DOTweenSettings Instance => _instanceCache ??= Resources.Load<DOTweenSettings>("DOTweenSettings");

        public SafeModeOptions safeModeOptions = new SafeModeOptions();
        public Ease defaultEaseType = Ease.OutQuad;
        public float defaultEaseOvershootOrAmplitude = 1.70158f;
        public float defaultEasePeriod = 0;

        // Editor-Only ► DOTween Inspector
        public bool showPlayingTweens, showPausedTweens;

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class SafeModeOptions
        {
            public NestedTweenFailureBehaviour nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
        }
    }
}