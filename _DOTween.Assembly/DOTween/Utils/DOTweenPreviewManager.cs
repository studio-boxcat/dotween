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

        public static bool IsPreviewing(int driver) => _tweens.ContainsKey(driver);

        public static void StartPreview(int driver, Tweener t)
        {
            L.I("[DOTweenPreviewManager] Start previewing tween: " + t);

            Assert.IsTrue(t.active, "Tween to preview must be active");

            if (_tweens.Count is 0)
            {
                L.I("[DOTweenPreviewManager] Starting AnimationMode");

                AnimationModeManager.Start();
                _lastUpdateTime = (float) EditorApplication.timeSinceStartup;
                EditorApplication.update += (_update ??= Update);
            }

            _tweens.Add(driver, t);

            TweenManager.DetachTween(t); // detach from update loop.
            t.SetAutoKill(true); // to make ForceUpdate() return true when the tween needs to be killed.
            t.OnStart(null).OnComplete(null).OnKill(null);
            t.Play();
        }

        public static bool TryStopPreview(int driver)
        {
            if (_tweens.Remove(driver, out var t) is false)
                return false;
            Internal_StopPreview(t);
            return true;
        }

        private static void StopPreview(Tweener t)
        {
            foreach (var (driver, someTween) in _tweens)
            {
                if (t != someTween) continue;
                _tweens.Remove(driver); // dictionary will be changed but it's okay since we will exit immediately.
                Internal_StopPreview(t);
                return;
            }

            L.E("[DOTweenPreviewManager] Given tween is not previewing: " + t);
        }

        private static void Internal_StopPreview(Tweener t)
        {
            Assert.IsTrue(t.active, "Tween must be active.");

            t.KillRewind();

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