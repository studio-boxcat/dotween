#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEditor;
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
                AnimationMode.StartAnimationMode(); // for screen refresh.
                _lastUpdateTime = (float) EditorApplication.timeSinceStartup;
                EditorApplication.update += (_update ??= Update);
                EditorApplication.playModeStateChanged += (_onPlayModeStateChanged ??= OnPlayModeStateChanged);
            }

            _tweens.Add(t.id, t);

            TweenManager.DetachTween(t); // detach from update loop.
            t.SetAutoKill(false);
            t.OnStart(null).OnComplete(null).OnKill(null);
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
            TweenManager.RestoreToOriginal(t);
            TweenManager.KillTween(t);

            if (_tweens.Count is 0)
            {
                AnimationMode.StopAnimationMode();
                EditorApplication.update -= _update;
            }
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
                tween.ForceUpdate(elapsed);
                if (tween.isComplete) tweenToKill.Add(tween);
            }

            foreach (var tween in tweenToKill)
                StopPreview(tween);
        }

        private static Action<PlayModeStateChange> _onPlayModeStateChanged;
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            while (_tweens.Count is not 0)
                StopPreview(_tweens[0]);
        }
    }
}
#endif