﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/09/20 17:40
//
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    /// <summary>
    /// Used to separate DOTween class from the MonoBehaviour instance (in order to use static constructors on DOTween).
    /// Contains all instance-based methods
    /// </summary>
    [AddComponentMenu("")]
    class DOTweenUnityBridge : MonoBehaviour
    {
        public static bool IsPlaying()
        {
#if UNITY_EDITOR
            return _editor_IsPlaying;
#else
            return Application.isPlaying;
#endif
        }

        static GameObject _instance;

        void Update() => DOTween.Update();

        void OnDestroy()
        {
            Assert.IsTrue(ReferenceEquals(_instance, gameObject), "Instance mismatch");
            _instance = null;
            DOTween.Clear();
        }

        internal static void Create()
        {
            Assert.IsTrue(Application.isPlaying, "Cannot create a DOTweenUnityBridge instance outside Play mode");
            Assert.IsNull(_instance, "An instance of DOTween is already running");
            _instance = new GameObject();
#if DEBUG
            _instance.name = nameof(DOTweenUnityBridge);
#endif
            DontDestroyOnLoad(_instance);
            _instance.AddComponent<DOTweenUnityBridge>();
        }

#if UNITY_EDITOR
        static bool _editor_IsPlaying;

        static DOTweenUnityBridge()
        {
            _editor_IsPlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;

            UnityEditor.EditorApplication.playModeStateChanged += state =>
            {
                if (state is UnityEditor.PlayModeStateChange.ExitingPlayMode)
                {
                    Assert.IsTrue(_editor_IsPlaying, "DOTweenUnityBridge is not playing");
                    _editor_IsPlaying = false;
                }
                else if (state is UnityEditor.PlayModeStateChange.ExitingEditMode)
                {
                    Assert.IsFalse(_editor_IsPlaying, "DOTweenUnityBridge is playing");
                    _editor_IsPlaying = true;
                }
            };
        }
#endif
    }
}