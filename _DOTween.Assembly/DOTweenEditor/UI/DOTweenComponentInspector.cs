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
        readonly StringBuilder _strb = new StringBuilder();
        bool _isRuntime;
        // Texture2D _headerImg;
        readonly GUIContent _gcPlay = new GUIContent("►");
        readonly GUIContent _gcPause = new GUIContent("❚❚");
        GUIContent _gcTitle;
        GUIContent _gcDebugModeSuggest = new GUIContent("Activate both Safe Mode and Debug Mode (including all checkboxes) " +
                                                        "in DOTween's preferences in order to " +
                                                        "allow this Inspector to give you <b>way more information</b> " +
                                                        "(like using a red cross to mark tweens with NULL targets)");

        #region Unity + GUI

        void OnEnable()
        {
            _isRuntime = EditorApplication.isPlaying;
            ConnectToSource(true);

            _strb.Length = 0;
            _strb.Append("DOTween v").Append(DOTween.Version);
            if (TweenManager.isDebugBuild) _strb.Append(" [Debug build]");
            else _strb.Append(" [Release build]");

            /*
            if (EditorUtils.hasPro) _strb.Append("\nDOTweenPro v").Append(EditorUtils.proVersion);
            else _strb.Append("\nDOTweenPro not installed");
            if (EditorUtils.hasDOTweenTimeline) _strb.Append("\nDOTweenTimeline v").Append(EditorUtils.dotweenTimelineVersion);
            else _strb.Append("\nDOTweenTimeline not installed");
            */
            _gcTitle = new GUIContent(_strb.ToString());
        }

        public override void OnInspectorGUI()
        {
            _isRuntime = EditorApplication.isPlaying;
            ConnectToSource();

            // Header img
            // GUILayout.Space(4);
            // GUILayout.BeginHorizontal();
            // Rect headeR = GUILayoutUtility.GetRect(0, 93, 18, 18);
            // GUI.DrawTexture(headeR, _headerImg, ScaleMode.ScaleToFit, true);
            GUILayout.Label(_isRuntime ? "RUNTIME MODE" : "EDITOR MODE");
            // GUILayout.FlexibleSpace();
            // GUILayout.EndHorizontal();

            GUILayout.Label(_gcTitle);
            if (!DOTween.useSafeMode || !DOTween.debugMode || !DOTween.debugStoreTargetId) {
                GUILayout.Label(_gcDebugModeSuggest);
            }

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Documentation")) Application.OpenURL("http://dotween.demigiant.com/documentation.php");
            if (GUILayout.Button("Check Updates")) Application.OpenURL("http://dotween.demigiant.com/download.php?v=" + DOTween.Version);
            GUILayout.EndHorizontal();

            if (_isRuntime) {
                int totActiveTweens = TweenManager.totActiveTweens;
                int totPlayingTweens = TweenManager.TotalPlayingTweens();
                int totPausedTweens = totActiveTweens - totPlayingTweens;
                int totActiveDefaultTweens = TweenManager.totActiveDefaultTweens;
                int totActiveLateTweens = TweenManager.totActiveLateTweens;
                int totActiveFixedTweens = TweenManager.totActiveFixedTweens;
                int totActiveManualTweens = TweenManager.totActiveManualTweens;

                GUILayout.Space(5);
                _strb.Length = 0;
                _strb.Append("Active tweens: ").Append(totActiveTweens)
                    .Append(" (").Append(TweenManager.totActiveTweeners).Append(" TW, ")
                    .Append(TweenManager.totActiveSequences).Append(" SE)")
                    .Append("\nDefault/Late/Fixed/Manual tweens: ").Append(totActiveDefaultTweens)
                    .Append("/").Append(totActiveLateTweens)
                    .Append("/").Append(totActiveFixedTweens)
                    .Append("/").Append(totActiveManualTweens);
                GUILayout.Label(_strb.ToString());

                GUILayout.Space(4);
                // DrawSimpleTweensList();
                DrawTweensButtons(totPlayingTweens, totPausedTweens);

                GUILayout.Space(2);
                _strb.Length = 0;
                _strb.Append("Pooled tweens: ").Append(TweenManager.TotalPooledTweens())
                    .Append(" (").Append(TweenManager.totPooledTweeners).Append(" TW, ")
                    .Append(TweenManager.totPooledSequences).Append(" SE)");
                GUILayout.Label(_strb.ToString());

                GUILayout.Space(2);
                _strb.Remove(0, _strb.Length);
                _strb.Append("Tweens Capacity: ").Append(TweenManager.maxTweeners).Append(" TW, ").Append(TweenManager.maxSequences).Append(" SE")
                    .Append("\nMax Simultaneous Active Tweens: ").Append(DOTween.maxActiveTweenersReached).Append(" TW, ")
                    .Append(DOTween.maxActiveSequencesReached).Append(" SE");
                GUILayout.Label(_strb.ToString());
            }

            GUILayout.Space(8);
            _strb.Remove(0, _strb.Length);
            _strb.Append("SETTINGS ▼");
            _strb.Append("\nSafe Mode: ").Append((_isRuntime ? DOTween.useSafeMode : _settings.useSafeMode) ? "ON" : "OFF");
            _strb.Append("\nTimeScale (Unity/DOTween/DOTween-Unscaled): ").Append(Time.timeScale)
                .Append("/").Append(_isRuntime ? DOTween.timeScale : _settings.timeScale)
                .Append("/").Append(_isRuntime ? DOTween.unscaledTimeScale : _settings.unscaledTimeScale);
            GUILayout.Label(_strb.ToString());
            GUILayout.Label(
                "NOTE: DOTween's TimeScale is not the same as Unity's Time.timeScale: it is actually multiplied by it except for tweens that are set to update independently");

            GUILayout.Space(8);
            _strb.Remove(0, _strb.Length);
            _strb.Append("DEFAULTS ▼");
            _strb.Append("\ndefaultRecyclable: ").Append(_isRuntime ? DOTween.defaultRecyclable : _settings.defaultRecyclable);
            _strb.Append("\ndefaultUpdateType: ").Append(_isRuntime ? DOTween.defaultUpdateType : _settings.defaultUpdateType);
            _strb.Append("\ndefaultTSIndependent: ").Append(_isRuntime ? DOTween.defaultTimeScaleIndependent : _settings.defaultTimeScaleIndependent);
            _strb.Append("\ndefaultEaseType: ").Append(_isRuntime ? DOTween.defaultEaseType : _settings.defaultEaseType);
            _strb.Append("\ndefaultLoopType: ").Append(_isRuntime ? DOTween.defaultLoopType : _settings.defaultLoopType);
            GUILayout.Label(_strb.ToString());

            GUILayout.Space(10);
        }

        #endregion

        #region Methods

        void ConnectToSource(bool forceReconnection = false)
        {
            // _headerImg = AssetDatabase.LoadAssetAtPath("Assets/" + EditorUtils.editorADBDir + "Imgs/DOTweenIcon.png", typeof(Texture2D)) as Texture2D;

            /*
            if (_settings == null || forceReconnection) {
                _settings = _isRuntime
                    ? Resources.Load(DOTweenSettings.AssetName) as DOTweenSettings
                    : DOTweenUtilityWindow.GetDOTweenSettings();
            }
            */
        }

        void DrawTweensButtons(int totPlayingTweens, int totPausedTweens)
        {
            _strb.Length = 0;
            _strb.Append("Playing tweens: ").Append(totPlayingTweens);
            _settings.showPlayingTweens = EditorGUILayout.Foldout(_settings.showPlayingTweens, _strb.ToString());
            if (_settings.showPlayingTweens) {
                foreach (Tween t in TweenManager._activeTweens) {
                    if (t == null || !t.isPlaying) continue;
                    DrawTweenButton(t, true);
                }
            }
            _strb.Length = 0;
            _strb.Append("Paused tweens: ").Append(totPausedTweens);
            _settings.showPausedTweens = EditorGUILayout.Foldout(_settings.showPausedTweens, _strb.ToString());
            if (_settings.showPausedTweens) {
                foreach (Tween t in TweenManager._activeTweens) {
                    if (t == null || t.isPlaying) continue;
                    DrawTweenButton(t, false);
                }
            }
        }

        void DrawTweenButton(Tween tween, bool isPlaying, bool isSequenced = false)
        {
            _strb.Length = 0;

            switch (tween.tweenType) {
            case TweenType.Tweener:
                if (!isSequenced) {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(isPlaying ? _gcPause : _gcPlay, GUILayout.Width(30))) {
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
                if (!isSequenced) {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(isPlaying ? _gcPause : _gcPlay, GUILayout.Width(30))) {
                        if (isPlaying) TweenManager.Pause(tween);
                        else TweenManager.Play(tween);
                    }
                }
                BuildTweenButtonLabel(tween, _strb);
                GUILayout.Label(_strb.ToString());
                if (!isSequenced) GUILayout.EndHorizontal();

                var s = (Sequence)tween;
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