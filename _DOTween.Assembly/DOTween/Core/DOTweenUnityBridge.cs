// Author: Daniele Giardini - http://www.demigiant.com
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
        static DOTweenUnityBridge _instance;

        void Update() => DOTween.Update();
        void OnApplicationQuit() => DOTween.Clear();

        internal static void Create()
        {
            Assert.IsNull(_instance, "An instance of DOTween is already running");
            var go = new GameObject("[DOTween]");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<DOTweenUnityBridge>();
        }

        internal static void DestroyInstance()
        {
            Assert.IsTrue(_instance is not null, "No instance of DOTween is running");
            _instance = null;
        }
    }
}