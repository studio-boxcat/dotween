using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.DOTweenEditor
{
    public static class DOTweenPreviewManager
    {
        static readonly Dictionary<int, Tweener> _tweens = new();
        static float _lastUpdateTime;

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

            t.SetUpdate(UpdateType.Manual);
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

        static void Internal_StopPreview(Tweener t)
        {
            if (_tweens.Count is 0)
                EditorApplication.update -= _update;

            TweenManager.RestoreToOriginal(t);
            TweenManager.Despawn(t);
        }

        static EditorApplication.CallbackFunction _update;
        static void Update()
        {
            Assert.IsTrue(_tweens.Count is not 0, "No tweens to update");

            var curTime = _lastUpdateTime;
            _lastUpdateTime = (float) EditorApplication.timeSinceStartup;
            var elapsed = _lastUpdateTime - curTime;
            DOTween.ManualUpdate(elapsed, elapsed);

            // Force visual refresh of UI objects
            // (a simple SceneView.RepaintAll won't work with UI elements)
            Canvas.ForceUpdateCanvases();
            InternalEditorUtility.RepaintAllViews();
        }

        static Action<PlayModeStateChange> _onPlayModeStateChanged;
        static void OnPlayModeStateChanged(PlayModeStateChange state)
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