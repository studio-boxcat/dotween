// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/03/12 16:03

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DG.DOTweenEditor
{
    [CustomEditor(typeof(DOTweenAnimation))]
    public class DOTweenAnimationInspector : Editor
    {
        static readonly Dictionary<DOTweenAnimation.AnimationType, Type[]> _eligibleTargetTypes = new()
        {
            { DOTweenAnimation.AnimationType.LocalMove, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.LocalRotate, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.Color, new[] { typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer), } },
            { DOTweenAnimation.AnimationType.Fade, new[] { typeof(CanvasGroup), typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer) } },
            { DOTweenAnimation.AnimationType.PunchPosition, new[] { typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimation.AnimationType.PunchRotation, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakePosition, new[] { typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakeRotation, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.UIAnchors, new[] { typeof(RectTransform) } },
        };

        DOTweenAnimation _src;
        int _totComponentsOnSrc; // Used to determine if a Component is added or removed from the source

        #region MonoBehaviour Methods

        void OnEnable()
        {
            _src = target as DOTweenAnimation;
        }

        static readonly List<Component> _componentBuf = new();

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("Animation Editor disabled while in play mode");
                return;
            }

            Undo.RecordObject(_src, "DOTween Animation");

            EditorGUIUtility.labelWidth = 100;


            // Preview in editor
            var previewId = _src.GetInstanceID();
            var wasPreviewing = DOTweenPreviewManager.IsPreviewing(previewId, out var previewingTween);
            var previewChanged = DrawPreview(wasPreviewing);
            if (previewChanged)
            {
                if (wasPreviewing is false)
                {
                    var t = _src.CreateTweenInstance();
                    t.id = previewId;
                    DOTweenPreviewManager.StartPreview(t);
                }
                else
                {
                    DOTweenPreviewManager.StopPreview(previewingTween);
                }
            }
            EditorGUILayout.Space(6);


            // Reset properties if the animation type changed.
            var prevAnimType = _src.animationType;
            _src.animationType = (DOTweenAnimation.AnimationType) EditorGUILayout.EnumPopup("Animation Type", _src.animationType);
            if (prevAnimType != _src.animationType)
            {
                // Set default optional values based on animation type
                switch (_src.animationType)
                {
                    case DOTweenAnimation.AnimationType.LocalMove:
                    case DOTweenAnimation.AnimationType.LocalRotate:
                    case DOTweenAnimation.AnimationType.Scale:
                        _src.endValueV3 = Vector3.zero;
                        _src.endValueFloat = 0;
                        _src.optionalBool0 = _src.animationType == DOTweenAnimation.AnimationType.Scale;
                        break;
                    case DOTweenAnimation.AnimationType.Color:
                    case DOTweenAnimation.AnimationType.Fade:
                        _src.endValueFloat = 0;
                        break;
                    case DOTweenAnimation.AnimationType.PunchPosition:
                    case DOTweenAnimation.AnimationType.PunchRotation:
                    case DOTweenAnimation.AnimationType.PunchScale:
                        _src.endValueV3 = _src.animationType == DOTweenAnimation.AnimationType.PunchRotation ? new Vector3(0, 180, 0) : Vector3.one;
                        _src.optionalFloat0 = 1;
                        _src.optionalInt0 = 10;
                        _src.optionalBool0 = false;
                        break;
                    case DOTweenAnimation.AnimationType.ShakePosition:
                    case DOTweenAnimation.AnimationType.ShakeRotation:
                    case DOTweenAnimation.AnimationType.ShakeScale:
                        _src.endValueV3 = _src.animationType == DOTweenAnimation.AnimationType.ShakeRotation ? new Vector3(90, 90, 90) : Vector3.one;
                        _src.optionalInt0 = 10;
                        _src.optionalFloat0 = 90;
                        _src.optionalBool0 = false;
                        _src.optionalBool1 = true;
                        break;
                    case DOTweenAnimation.AnimationType.UIAnchors:
                        _src.endValueV3 = new Vector3(0.5f, 0.5f, 0);
                        break;
                }
            }

            if (_src.animationType == DOTweenAnimation.AnimationType.None)
            {
                if (GUI.changed) EditorUtility.SetDirty(_src);
                return;
            }


            // Draw the target selector.
            CollectMatchingTargets(_src.gameObject, _src.animationType, _componentBuf);
            var newTarget = _componentBuf.Count is not 1
                ? ComponentSelector("Target", _src.target, _componentBuf)
                : _componentBuf[0];
            _componentBuf.Clear();
            if (ReferenceEquals(_src.target, newTarget) is false)
                _src.target = newTarget;


            // Draw Duration & Delay.
            EditorGUILayout.BeginHorizontal();
            _src.duration = EditorGUILayout.FloatField("Duration", _src.duration);
            if (_src.duration < 0) _src.duration = 0;
            EditorGUILayout.Space(18, false);
            _src.delay = EditorGUILayout.FloatField("Delay", _src.delay);
            if (_src.delay < 0) _src.delay = 0;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(6);


            // Draw the rest of the properties.
            bool canBeRelative = true;
            // End value and eventual specific options
            switch (_src.animationType)
            {
                case DOTweenAnimation.AnimationType.LocalMove:
                    GUIEndValueV3();
                    break;
                case DOTweenAnimation.AnimationType.LocalRotate:
                    GUIEndValueV3();
                    _src.optionalRotationMode = (byte) EditorGUILayout.IntField("    Rotation Mode", _src.optionalRotationMode);
                    break;
                case DOTweenAnimation.AnimationType.Scale:
                    if (_src.optionalBool0) GUIEndValueFloat();
                    else GUIEndValueV3();
                    _src.optionalBool0 = EditorGUILayout.Toggle("Uniform Scale", _src.optionalBool0);
                    break;
                case DOTweenAnimation.AnimationType.Color:
                    GUIEndValueColor();
                    canBeRelative = false;
                    break;
                case DOTweenAnimation.AnimationType.Fade:
                    GUIEndValueFloat();
                    if (_src.endValueFloat < 0) _src.endValueFloat = 0;
                    canBeRelative = false;
                    break;
                case DOTweenAnimation.AnimationType.PunchPosition:
                case DOTweenAnimation.AnimationType.PunchRotation:
                case DOTweenAnimation.AnimationType.PunchScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the punch vibrate"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Elasticity", "How much the vector will go beyond the starting position when bouncing backwards"), _src.optionalFloat0, 0, 1);
                    break;
                case DOTweenAnimation.AnimationType.ShakePosition:
                case DOTweenAnimation.AnimationType.ShakeRotation:
                case DOTweenAnimation.AnimationType.ShakeScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the shake vibrate"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Randomness", "The shake randomness"), _src.optionalFloat0, 0, 90);
                    _src.optionalBool1 = EditorGUILayout.Toggle(new GUIContent("    FadeOut", "If selected the shake will fade out, otherwise it will constantly play with full force"), _src.optionalBool1);
                    break;
                case DOTweenAnimation.AnimationType.UIAnchors:
                    GUIEndValueV2();
                    canBeRelative = false;
                    break;
            }

            // Final settings
            if (canBeRelative) _src.isRelative = EditorGUILayout.Toggle("    Relative", _src.isRelative);

            _src.easeType = (Ease) EditorGUILayout.EnumPopup("Ease", _src.easeType);
            if (_src.easeType == Ease.INTERNAL_Custom)
                _src.easeCurve = EditorGUILayout.CurveField("   Ease Curve", _src.easeCurve);
            _src.loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), _src.loops);
            if (_src.loops < -1) _src.loops = -1;
            if (_src.loops is > 1 or -1)
                _src.loopType = (LoopType) EditorGUILayout.EnumPopup("   Loop Type", _src.loopType);

            EditorGUI.EndDisabledGroup();
            GUILayout.Space(6);


            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 70;
            _src.autoGenerate = EditorGUILayout.ToggleLeft(new GUIContent("AutoGenerate", "If selected, the tween will be generated at startup (during Start for RectTransform position tween, Awake for all the others)"), _src.autoGenerate);
            if (_src.autoGenerate)
                _src.autoPlay = EditorGUILayout.ToggleLeft(new GUIContent("AutoPlay", "If selected, the tween will play automatically"), _src.autoPlay);
            _src.autoKill = EditorGUILayout.ToggleLeft(new GUIContent("AutoKill", "If selected, the tween will be killed when it completes, and won't be reusable"), _src.autoKill);
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 110;
            GUILayout.EndHorizontal();


            if (GUI.changed) EditorUtility.SetDirty(_src);
        }

        #endregion

        /// <summary>
        /// Returns TRUE if its actually previewing animations
        /// </summary>
        public static bool DrawPreview(bool previewing)
        {
            if (EditorApplication.isPlaying)
                return false;

            // Preview - Play
            if (previewing is false)
            {
                if (GUILayout.Button("► Play"))
                    return true;
            }
            // Preview - Stop
            else
            {
                if (GUILayout.Button("■ Stop"))
                    return true;
            }

            return false;
        }

        #region Methods

        static void CollectMatchingTargets(GameObject targetGO, DOTweenAnimation.AnimationType animType, List<Component> result)
        {
            var types = _eligibleTargetTypes[animType];
            foreach (var t in types)
            {
                if (targetGO.TryGetComponent(t, out var targetComp)
                    && result.Contains(targetComp) is false)
                {
                    result.Add(targetComp);
                }
            }
        }

        static Component ComponentSelector(string label, Component cur, List<Component> components)
        {
            var count = components.Count;
            var options = new string[count];
            for (var i = 0; i < count; i++)
                options[i] = components[i].GetType().Name;
            var index = Array.IndexOf(components.ToArray(), cur);
            if (index is -1) index = 0;
            var newIndex = EditorGUILayout.Popup(label, index, options);
            return components[newIndex];
        }

        #endregion

        #region GUI Draw Methods

        void GUIEndValueFloat()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueFloat = EditorGUILayout.FloatField(_src.endValueFloat);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueColor()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueColor = EditorGUILayout.ColorField(_src.endValueColor);
            GUILayout.EndHorizontal();
        }

        void GUIEndValueV2()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueV3 = EditorGUILayout.Vector2Field("", _src.endValueV3, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        void GUIEndValueV3()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueV3 = EditorGUILayout.Vector3Field("", _src.endValueV3, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        void GUIToFromButton()
        {
            if (GUILayout.Button(_src.isFrom ? "FROM" : "TO", GUILayout.Width(90))) _src.isFrom = !_src.isFrom;
            GUILayout.Space(EditorGUIUtility.labelWidth - 90);
        }

        #endregion
    }
}