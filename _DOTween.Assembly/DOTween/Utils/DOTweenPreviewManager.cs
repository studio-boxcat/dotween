#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Assertions;

namespace DG.Tweening
{
    public static class DOTweenPreviewManager
    {
        private static readonly Dictionary<int, Tweener> _tweens = new();
        private static float _lastUpdateTime;

        public static bool IsPreviewing(int id) => _tweens.ContainsKey(id);
        public static bool IsPreviewing(int id, out Tweener t) => _tweens.TryGetValue(id, out t);

        public static void StartPreview(Tweener t)
        {
            L.I("[DOTweenPreviewManager] Start previewing tween: " + t);

            Assert.AreNotEqual(Tween.invalidId, t.id, "Tween to preview must have a valid id");

            if (_tweens.Count is 0)
            {
                L.I("[DOTweenPreviewManager] Starting AnimationMode");

                AnimationModeManager.Start();
                _lastUpdateTime = (float) EditorApplication.timeSinceStartup;
                EditorApplication.update += (_update ??= Update);
            }

            _tweens.Add(t.id, t);

            TweenManager.DetachTween(t); // detach from update loop.
            t.SetAutoKill(true); // to make ForceUpdate() return true when the tween needs to be killed.
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
                L.I("[DOTweenPreviewManager] Stopping AnimationMode");

                AnimationModeManager.Stop();
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
                if (tween.ForceUpdate(elapsed))
                    tweenToKill.Add(tween);
            }

            foreach (var tween in tweenToKill)
                StopPreview(tween);

            // force repaint the SceneView. Without this, the SceneView renders occasionally.
            SceneView.RepaintAll();
        }

        [PlayModeGate]
        private static void StopAllPreviews()
        {
            while (_tweens.NotEmpty())
                StopPreview(_tweens[0]);
        }
    }
}
#endif