using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.DOTweenEditor.UI
{
    [CustomEditor(typeof(DOTweenUnityBridge))]
    public class DOTweenComponentInspector : Editor
    {
        readonly StringBuilder _sb = new();
        readonly GUIContent _gcPlay = new("►");
        readonly GUIContent _gcPause = new("❚❚");

        #region Unity + GUI

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying is false)
                return;

            DrawTweensButtons();
            GUILayout.Space(2);

            _sb.Clear();
            _sb.Append("Pooled tweens")
                .Append(" (").Append(TweenPool.SumPooledTweeners()).Append(" TW, ")
                .Append(TweenPool.SumPooledSequences()).Append(" SE)");
            GUILayout.Label(_sb.ToString());
        }

        #endregion

        #region Methods

        void DrawTweensButtons()
        {
            var tweens = TweenManager.Tweens.StartIterate(out var lastUpdateId);

            GUILayout.Label("Playing tweens");
            foreach (var t in tweens)
            {
                if (t.updateId.IsInvalid()) continue;
                if (t.updateId > lastUpdateId) break;
                Assert.IsTrue(t.active, "Tween is not active");

                if (t.isPlaying)
                    DrawTweenButton(t, true);
            }

            GUILayout.Label("Paused tweens");
            foreach (var t in tweens)
            {
                if (t.updateId.IsInvalid()) continue;
                if (t.updateId > lastUpdateId) break;
                Assert.IsTrue(t.active, "Tween is not active");

                if (t.isPlaying is false)
                    DrawTweenButton(t, false);
            }

            TweenManager.Tweens.EndIterate();
        }

        void DrawTweenButton(Tween tween, bool isPlaying, bool isSequenced = false)
        {
            _sb.Length = 0;

            switch (tween.tweenType)
            {
                case TweenType.Tweener:
                    if (!isSequenced)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(isPlaying ? _gcPause : _gcPlay, GUILayout.Width(30)))
                        {
                            if (isPlaying) TweenManager.Pause(tween);
                            else TweenManager.Play(tween);
                        }
                    }

                    if (tween.target is Object obj && obj != null)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(obj, obj.GetType(), true);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        BuildTweenButtonLabel(tween, _sb);
                        GUILayout.Label(_sb.ToString());
                    }

                    if (!isSequenced) GUILayout.EndHorizontal();
                    break;
                case TweenType.Sequence:
                    if (!isSequenced)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(isPlaying ? _gcPause : _gcPlay, GUILayout.Width(30)))
                        {
                            if (isPlaying) TweenManager.Pause(tween);
                            else TweenManager.Play(tween);
                        }
                    }
                    BuildTweenButtonLabel(tween, _sb);
                    GUILayout.Label(_sb.ToString());
                    if (!isSequenced) GUILayout.EndHorizontal();

                    var s = (Sequence) tween;
                    foreach (var t in s.sequencedTweens)
                        DrawTweenButton(t, isPlaying, true);
                    break;
            }
        }

        #endregion

        #region Helpers

        static void BuildTweenButtonLabel(Tween t, StringBuilder sb)
        {
            if (t.tweenType == TweenType.Sequence)
                sb.Append("[SEQUENCE] ");

            if (string.IsNullOrEmpty(t.debugHint) == false)
                sb.Append(t.debugHint).Append(';');
            if (t.id != Tween.invalidId)
                sb.Append(t.id).Append(";");
            sb.Append(t.target ?? "null");
        }

        #endregion
    }
}