// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/09/20 17:40
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    /// <summary>
    /// Used to separate DOTween class from the MonoBehaviour instance (in order to use static constructors on DOTween).
    /// Contains all instance-based methods
    /// </summary>
    [AddComponentMenu("")]
    public class DOTweenComponent : MonoBehaviour
    {
        float _unscaledTime;

        bool _paused; // Used to mark when app is paused and to avoid resume being called when application starts playing
        float _pausedTime; // Marks the time when Unity was paused
        bool _isQuitting;

        bool _duplicateToDestroy;

        #region Unity Methods

        void Awake()
        {
            if (DOTween.instance == null) DOTween.instance = this;
            else {
                if (Debugger.logPriority >= 1) {
                    Debugger.LogWarning("Duplicate DOTweenComponent instance found in scene: destroying it");
                }
                Destroy(this.gameObject);
                return;
            }

            _unscaledTime = Time.realtimeSinceStartup;

            // From DOTweenModuleUtils.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged +=
                _ => OnApplicationPause(UnityEditor.EditorApplication.isPaused);
#endif
        }

        void Start()
        {
            // Check if there's a leftover persistent DOTween object
            // (should be impossible but some weird Unity freeze caused that to happen on Seith's project
            if (DOTween.instance != this) {
                _duplicateToDestroy = true;
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            if (TweenManager.hasActiveDefaultTweens) {
                var unscaledDeltaTime = Time.realtimeSinceStartup - _unscaledTime;
                TweenManager.Update(UpdateType.Normal, Time.deltaTime, unscaledDeltaTime);
            }
            _unscaledTime = Time.realtimeSinceStartup;
        }

        void OnDestroy()
        {
            if (_duplicateToDestroy) return;

//            DOTween.initialized = false;
//            DOTween.instance = null;

            if (DOTween.instance == this) DOTween.instance = null;
            DOTween.Clear(true, _isQuitting);
        }

        // Detract/reapply pause time from/to unscaled time
        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) {
                _paused = true;
                _pausedTime = Time.realtimeSinceStartup;
            } else if (_paused) {
                _paused = false;
                _unscaledTime += Time.realtimeSinceStartup - _pausedTime;
            }
        }

        void OnApplicationQuit()
        {
            _isQuitting = true;
            DOTween.isQuitting = true;
        }

        #endregion

        internal static void Create()
        {
            if (DOTween.instance != null) return;

            var go = new GameObject("[DOTween]");
            DontDestroyOnLoad(go);
            DOTween.instance = go.AddComponent<DOTweenComponent>();
        }

        internal static void DestroyInstance()
        {
            if (DOTween.instance != null) Destroy(DOTween.instance.gameObject);
            DOTween.instance = null;
        }
    }
}