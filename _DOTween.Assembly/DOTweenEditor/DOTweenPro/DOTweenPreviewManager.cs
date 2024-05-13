// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/03/12 16:03

using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Logger = DG.Tweening.Logger;

namespace DG.DOTweenEditor
{
    public static class DOTweenPreviewManager
    {
        static readonly Dictionary<DOTweenAnimation, Tweener> _AnimationToTween = new();
        static readonly List<DOTweenAnimation> _TmpKeys = new();

        #region Public Methods & GUI

        /// <summary>
        /// Returns TRUE if its actually previewing animations
        /// </summary>
        public static bool PreviewGUI(DOTweenAnimation src)
        {
            if (EditorApplication.isPlaying) return false;

            var isPreviewing = _AnimationToTween.Count > 0;
            var isPreviewingThis = isPreviewing && _AnimationToTween.ContainsKey(src);

            // Preview - Play
            GUILayout.BeginHorizontal();
            if (isPreviewingThis == false)
            {
                EditorGUI.BeginDisabledGroup(
                    src.animationType == DOTweenAnimation.AnimationType.None);
                if (GUILayout.Button("► Play")) {
                    if (!isPreviewing) StartupGlobalPreview();
                    AddAnimationToGlobalPreview(src);
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(isPreviewing);
                if (GUILayout.Button("► Play (GameObject)")) {
                    if (!isPreviewing) StartupGlobalPreview();
                    DOTweenAnimation[] anims = src.gameObject.GetComponents<DOTweenAnimation>();
                    foreach (DOTweenAnimation anim in anims) AddAnimationToGlobalPreview(anim);
                }
                EditorGUI.EndDisabledGroup();
            }
            // Preview - Stop
            else
            {
                if (GUILayout.Button("■ Stop")) {
                    if (_AnimationToTween.TryGetValue(src, out var tween))
                        StopPreview(tween);
                }
                if (GUILayout.Button("■ Stop (GameObject)")) {
                    StopPreview(src.gameObject);
                }
            }
            GUILayout.EndHorizontal();

            return isPreviewing;
        }

        static void StopAllPreviews(PlayModeStateChange state)
        {
            StopAllPreviews();
        }

        public static void StopAllPreviews()
        {
            _TmpKeys.Clear();
            _TmpKeys.AddRange(_AnimationToTween.Keys);
            StopPreview(_TmpKeys);
            _TmpKeys.Clear();
            _AnimationToTween.Clear();

            DOTweenEditorPreview.Stop();
            EditorApplication.playModeStateChanged -= StopAllPreviews;
            InternalEditorUtility.RepaintAllViews();
        }

#endregion

#region Methods

        static void StartupGlobalPreview()
        {
            DOTweenEditorPreview.Start();
            EditorApplication.playModeStateChanged += StopAllPreviews;
        }

        static void AddAnimationToGlobalPreview(DOTweenAnimation src)
        {
            var t = src.CreateEditorPreview();
            if (t == null) return;
            _AnimationToTween.Add(src, t);
            DOTweenEditorPreview.PrepareTweenForPreview(t);
        }

        static void StopPreview(GameObject go)
        {
            _TmpKeys.Clear();
            foreach (var kvp in _AnimationToTween) {
                if (kvp.Key.gameObject != go) continue;
                _TmpKeys.Add(kvp.Key);
            }
            StopPreview(_TmpKeys);
            _TmpKeys.Clear();

            if (_AnimationToTween.Count == 0) StopAllPreviews();
            else InternalEditorUtility.RepaintAllViews();
        }

        static void StopPreview(Tweener t)
        {
            DOTweenAnimation anim = null;
            foreach (var (curAnim, curTween) in _AnimationToTween) {
                if (curTween != t) continue;
                anim = curAnim;
                _AnimationToTween.Remove(curAnim);
                break;
            }
            if (anim is null) {
                Logger.Warning("DOTween Preview ► Couldn't find tween to stop");
                return;
            }
            t.KillRewind();
            EditorUtility.SetDirty(anim); // Refresh views

            if (_AnimationToTween.Count == 0) StopAllPreviews();
            else InternalEditorUtility.RepaintAllViews();
        }

        // Stops while iterating inversely, which deals better with tweens that overwrite each other
        static void StopPreview(List<DOTweenAnimation> keys)
        {
            for (var i = keys.Count - 1; i > -1; --i) {
                var anim = keys[i];
                var tween = _AnimationToTween[anim];
                tween.KillRewind();
                EditorUtility.SetDirty(anim); // Refresh views
                _AnimationToTween.Remove(anim);
            }
        }

#endregion
    }
}
