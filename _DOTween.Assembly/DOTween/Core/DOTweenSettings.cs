// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/02/05 10:28

using System;
using DG.Tweening.Core.Enums;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    public class DOTweenSettings : ScriptableObject
    {
        static DOTweenSettings _instanceCache = null;
        public static DOTweenSettings Instance => _instanceCache ??= Resources.Load<DOTweenSettings>("DOTweenSettings");

        public bool useSafeMode = true;
        public SafeModeOptions safeModeOptions = new SafeModeOptions();
        public float timeScale = 1;
        public float unscaledTimeScale = 1;
        public bool useSmoothDeltaTime;
        public float maxSmoothUnscaledTime = 0.15f; // Used if useSmoothDeltaTime is TRUE
        public bool showUnityEditorReport;
        public LogBehaviour logBehaviour = LogBehaviour.Default;
        public bool drawGizmos = true;
        public bool defaultRecyclable;
        public AutoPlay defaultAutoPlay = AutoPlay.All;
        public UpdateType defaultUpdateType;
        public bool defaultTimeScaleIndependent;
        public Ease defaultEaseType = Ease.OutQuad;
        public float defaultEaseOvershootOrAmplitude = 1.70158f;
        public float defaultEasePeriod = 0;
        public bool defaultAutoKill = true;
        public LoopType defaultLoopType = LoopType.Restart;

        // Debug
        public bool debugMode = false;
        // Stores the target id so it can be used to give more info in case of safeMode error capturing
        public bool debugStoreTargetId = true;

        // Editor-Only ► DOTween Inspector
        public bool showPlayingTweens, showPausedTweens;

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class SafeModeOptions
        {
            public SafeModeLogBehaviour logBehaviour = SafeModeLogBehaviour.Warning;
            public NestedTweenFailureBehaviour nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
        }
    }
}