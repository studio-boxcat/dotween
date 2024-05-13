#if UNITY_EDITOR
// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/06/29 20:37
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor.UI
{
    [CustomEditor(typeof(DOTweenComponent))]
    public class DOTweenComponentInspector : Editor
    {
        static DOTweenSettings _settings => DOTweenSettings.Instance;
        readonly StringBuilder _strb = new();
        readonly GUIContent _gcPlay = new("►");
        readonly GUIContent _gcPause = new("❚❚");
        GUIContent _gcTitle;

        #region Unity + GUI

        void OnEnable()
        {
            _gcTitle = new GUIContent("DOTween v" + DOTween.Version);
        }

        public override void OnInspectorGUI()
        {
            var playing = EditorApplication.isPlaying;

            GUILayout.Label(playing ? "RUNTIME MODE" : "EDITOR MODE");
            GUILayout.Label(_gcTitle);

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check Updates")) Application.OpenURL("http://dotween.demigiant.com/download.php?v=" + DOTween.Version);
            GUILayout.EndHorizontal();

            if (playing)
            {
                int totActiveTweens = TweenManager.totActiveTweens;
                int totPlayingTweens = TweenManager.TotalPlayingTweens();
                int totPausedTweens = totActiveTweens - totPlayingTweens;
                int totActiveDefaultTweens = TweenManager.totActiveDefaultTweens;
                int totActiveManualTweens = TweenManager.totActiveManualTweens;

                GUILayout.Space(5);
                _strb.Clear();
                _strb.Append("Active tweens: ").Append(totActiveTweens)
                    .Append(" (").Append(TweenManager.totActiveTweeners).Append(" TW, ")
                    .Append(TweenManager.totActiveSequences).Append(" SE)")
                    .Append("\nDefault/Manual tweens: ").Append(totActiveDefaultTweens)
                    .Append("/").Append(totActiveManualTweens);
                GUILayout.Label(_strb.ToString());

                GUILayout.Space(4);
                DrawTweensButtons(totPlayingTweens, totPausedTweens);

                GUILayout.Space(2);
                _strb.Clear();
                _strb.Append("Pooled tweens: ").Append(TweenManager.TotalPooledTweens())
                    .Append(" (").Append(TweenManager.totPooledTweeners).Append(" TW, ")
                    .Append(TweenManager.totPooledSequences).Append(" SE)");
                GUILayout.Label(_strb.ToString());

                GUILayout.Space(2);
                _strb.Clear();
                _strb.Append("Tweens Capacity: ").Append(TweenManager.maxTweeners).Append(" TW, ").Append(TweenManager.maxSequences).Append(" SE")
                    .Append("\nMax Simultaneous Active Tweens: ").Append(DOTween.maxActiveTweenersReached).Append(" TW, ")
                    .Append(DOTween.maxActiveSequencesReached).Append(" SE");
                GUILayout.Label(_strb.ToString());
            }

            GUILayout.Space(8);
            _strb.Clear();
            _strb.Append("SETTINGS ▼");
            _strb.Append("\nSafe Mode: ").Append((playing ? DOTween.useSafeMode : _settings.useSafeMode) ? "ON" : "OFF");
            _strb.Append("\ndefaultEaseType: ").Append(playing ? DOTween.defaultEaseType : _settings.defaultEaseType);
            GUILayout.Label(_strb.ToString());

            GUILayout.Space(10);
        }

        #endregion

        #region Methods

        void DrawTweensButtons(int totPlayingTweens, int totPausedTweens)
        {
            _strb.Length = 0;
            _strb.Append("Playing tweens: ").Append(totPlayingTweens);
            _settings.showPlayingTweens = EditorGUILayout.Foldout(_settings.showPlayingTweens, _strb.ToString());
            if (_settings.showPlayingTweens)
            {
                foreach (Tween t in TweenManager._activeTweens)
                {
                    if (t == null || !t.isPlaying) continue;
                    DrawTweenButton(t, true);
                }
            }
            _strb.Length = 0;
            _strb.Append("Paused tweens: ").Append(totPausedTweens);
            _settings.showPausedTweens = EditorGUILayout.Foldout(_settings.showPausedTweens, _strb.ToString());
            if (_settings.showPausedTweens)
            {
                foreach (Tween t in TweenManager._activeTweens)
                {
                    if (t == null || t.isPlaying) continue;
                    DrawTweenButton(t, false);
                }
            }
        }

        void DrawTweenButton(Tween tween, bool isPlaying, bool isSequenced = false)
        {
            _strb.Length = 0;

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
                        BuildTweenButtonLabel(tween, _strb);
                        GUILayout.Label(_strb.ToString());
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
                    BuildTweenButtonLabel(tween, _strb);
                    GUILayout.Label(_strb.ToString());
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

            if (string.IsNullOrEmpty(t.debugTargetId) == false)
                sb.Append(t.debugTargetId).Append(';');
            if (t.id != Tween.invalidId)
                sb.Append(t.id).Append(";");
            sb.Append(t.target ?? "null");
        }

        #endregion
    }
}
#endif