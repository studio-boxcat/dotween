#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public static class DOTweenPreviewManager
    {
        private static readonly Dictionary<int, Tweener> _tweens = new();
        private static float _lastUpdateTime;

        public static bool IsPreviewing(int id, out Tweener t)
        {
            return _tweens.TryGetValue(id, out t);
        }

        public static void StartPreview(Tweener t)
        {
            Assert.AreNotEqual(Tween.invalidId, t.id, "Tween to preview must have a valid id");

            if (_tweens.Count is 0)
            {
                _lastUpdateTime = (float) EditorApplication.timeSinceStartup;
                EditorApplication.update += (_update ??= Update);
                EditorApplication.playModeStateChanged += (_onPlayModeStateChanged ??= OnPlayModeStateChanged);
            }

            _tweens.Add(t.id, t);

            TweenManager.DetachTween(t);
            t.SetAutoKill(false);
            t.OnStart(null).OnUpdate(null)
                .OnComplete(null).OnKill(null);
            t.Play();
        }

        public static bool TryStopPreview(int id)
        {
            if (_tweens.Remove(id, out var t) is false)
                return false;
            Internal_StopPreview(t);
            return true;
        }

        public static void StopPreview(Tweener t)
        {
            var removed = _tweens.Remove(t.id, out var oldTween);
            Assert.IsTrue(removed, "Tween to stop preview not found");
            Assert.AreEqual(t, oldTween, "Tween to stop preview is not the same as the one playing");
            Internal_StopPreview(t);
        }

        private static void Internal_StopPreview(Tweener t)
        {
            if (_tweens.Count is 0)
                EditorApplication.update -= _update;

            TweenManager.RestoreToOriginal(t);
            TweenManager.KillTween(t);
        }

        private static EditorApplication.CallbackFunction _update;
        private static void Update()
        {
            Assert.IsTrue(_tweens.Count is not 0, "No tweens to update");

            var curTime = _lastUpdateTime;
            _lastUpdateTime = (float) EditorApplication.timeSinceStartup;
            var elapsed = _lastUpdateTime - curTime;

            var tweenToKill = new List<Tweener>();
            foreach (var tween in _tweens.Values)
            {
                var needKill = tween.ForceUpdate(elapsed);
                if (needKill) tweenToKill.Add(tween);
            }

            foreach (var tween in tweenToKill)
                StopPreview(tween);

            // Force visual refresh of UI objects
            // (a simple SceneView.RepaintAll won't work with UI elements)
            Canvas.ForceUpdateCanvases();
            InternalEditorUtility.RepaintAllViews();
        }

        private static Action<PlayModeStateChange> _onPlayModeStateChanged;
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            EditorApplication.playModeStateChanged -= _onPlayModeStateChanged;

            if (_tweens.Count is not 0)
            {
                foreach (var t in _tweens.Values)
                    Internal_StopPreview(t);
                _tweens.Clear();
                EditorApplication.update -= _update;
            }
        }
    }
}
#endif