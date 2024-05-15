﻿using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.DOTweenEditor.UI
{
    [CustomEditor(typeof(DOTweenUnityBridge))]
    public class DOTweenComponentInspector : Editor
    {
        readonly StringBuilder _sb = new();

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
            var tweens = TweenManager.Tweens.StartIterate();
            foreach (var t in tweens)
                DrawTweenButton(t);
            TweenManager.Tweens.EndIterate();
        }

        void DrawTweenButton(Tween tween, bool isSequenced = false)
        {
            _sb.Length = 0;

            if (tween is Tweener)
            {
                if (!isSequenced)
                {
                    GUILayout.BeginHorizontal();
                    DrawPlayToggle(tween);
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
            }
            else if (tween is Sequence s)
            {
                if (!isSequenced)
                {
                    GUILayout.BeginHorizontal();
                    DrawPlayToggle(s);
                }
                BuildTweenButtonLabel(s, _sb);
                GUILayout.Label(_sb.ToString());
                if (!isSequenced) GUILayout.EndHorizontal();

                foreach (var t in s.sequencedTweens)
                    DrawTweenButton(t);
            }
        }

        static readonly GUIContent _gcPlay = new("►");
        static readonly GUIContent _gcPause = new("❚❚");

        static void DrawPlayToggle(Tween tween)
        {
            var isPlaying = tween.isPlaying;
            if (GUILayout.Button(isPlaying ? _gcPause : _gcPlay, GUILayout.Width(30)))
            {
                if (isPlaying) TweenManager.Pause(tween);
                else TweenManager.Play(tween);
            }
        }

        #endregion

        #region Helpers

        static void BuildTweenButtonLabel(Tween t, StringBuilder sb)
        {
            if (t is Sequence)
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