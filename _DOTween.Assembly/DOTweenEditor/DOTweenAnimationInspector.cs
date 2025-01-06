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
        private static readonly Dictionary<DOTweenAnimationType, Type[]> _eligibleTargetTypes = new()
        {
            { DOTweenAnimationType.LocalMove, new[] { typeof(Transform) } },
            { DOTweenAnimationType.LocalRotateZ, new[] { typeof(Transform) } },
            { DOTweenAnimationType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimationType.Color, new[] { typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer), } },
            { DOTweenAnimationType.Fade, new[] { typeof(CanvasGroup), typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer) } },
            { DOTweenAnimationType.PunchPosition, new[] { typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimationType.PunchRotation, new[] { typeof(Transform) } },
            { DOTweenAnimationType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimationType.ShakePosition, new[] { typeof(RectTransform), typeof(Transform) } },
            { DOTweenAnimationType.ShakeRotation, new[] { typeof(Transform) } },
            { DOTweenAnimationType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimationType.UIAnchors, new[] { typeof(RectTransform) } },
        };

        private DOTweenAnimation _src;
        private int _totComponentsOnSrc; // Used to determine if a Component is added or removed from the source

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            _src = target as DOTweenAnimation;
        }

        private static readonly List<Component> _componentBuf = new();

        public override void OnInspectorGUI()
        {
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
            _src.animationType = (DOTweenAnimationType) EditorGUILayout.EnumPopup("Animation Type", _src.animationType);
            if (prevAnimType != _src.animationType)
            {
                // Set default optional values based on animation type
                switch (_src.animationType)
                {
                    case DOTweenAnimationType.LocalMove:
                    case DOTweenAnimationType.LocalRotateZ:
                    case DOTweenAnimationType.Scale:
                        _src.endValueV3 = Vector3.zero;
                        _src.endValueFloat = 0;
                        _src.optionalBool0 = _src.animationType == DOTweenAnimationType.Scale;
                        break;
                    case DOTweenAnimationType.Color:
                    case DOTweenAnimationType.Fade:
                        _src.endValueFloat = 0;
                        break;
                    case DOTweenAnimationType.PunchPosition:
                    case DOTweenAnimationType.PunchRotation:
                    case DOTweenAnimationType.PunchScale:
                        _src.endValueV3 = _src.animationType == DOTweenAnimationType.PunchRotation ? new Vector3(0, 180, 0) : Vector3.one;
                        _src.optionalFloat0 = 1;
                        _src.optionalInt0 = 10;
                        _src.optionalBool0 = false;
                        break;
                    case DOTweenAnimationType.ShakePosition:
                    case DOTweenAnimationType.ShakeRotation:
                    case DOTweenAnimationType.ShakeScale:
                        _src.endValueV3 = _src.animationType == DOTweenAnimationType.ShakeRotation ? new Vector3(90, 90, 90) : Vector3.one;
                        _src.optionalInt0 = 10;
                        _src.optionalFloat0 = 90;
                        _src.optionalBool0 = false;
                        _src.optionalBool1 = true;
                        break;
                    case DOTweenAnimationType.UIAnchors:
                        _src.endValueV3 = new Vector3(0.5f, 0.5f, 0);
                        break;
                }
            }

            if (_src.animationType == DOTweenAnimationType.None)
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
                case DOTweenAnimationType.LocalMove:
                    GUIEndValueV3();
                    break;
                case DOTweenAnimationType.LocalRotateZ:
                    GUIEndValueZ();
                    break;
                case DOTweenAnimationType.Scale:
                    if (_src.optionalBool0) GUIEndValueFloat();
                    else GUIEndValueV3();
                    _src.optionalBool0 = EditorGUILayout.Toggle("Uniform Scale", _src.optionalBool0);
                    break;
                case DOTweenAnimationType.Color:
                    GUIEndValueColor();
                    canBeRelative = false;
                    break;
                case DOTweenAnimationType.Fade:
                    GUIEndValueFloat();
                    if (_src.endValueFloat < 0) _src.endValueFloat = 0;
                    canBeRelative = false;
                    break;
                case DOTweenAnimationType.PunchPosition:
                case DOTweenAnimationType.PunchRotation:
                case DOTweenAnimationType.PunchScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the punch vibrate"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Elasticity", "How much the vector will go beyond the starting position when bouncing backwards"), _src.optionalFloat0, 0, 1);
                    break;
                case DOTweenAnimationType.ShakePosition:
                case DOTweenAnimationType.ShakeRotation:
                case DOTweenAnimationType.ShakeScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the shake vibrate"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Randomness", "The shake randomness"), _src.optionalFloat0, 0, 90);
                    _src.optionalBool1 = EditorGUILayout.Toggle(new GUIContent("    FadeOut", "If selected the shake will fade out, otherwise it will constantly play with full force"), _src.optionalBool1);
                    break;
                case DOTweenAnimationType.UIAnchors:
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
        private static bool DrawPreview(bool previewing)
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

        private static void CollectMatchingTargets(GameObject targetGO, DOTweenAnimationType animType, List<Component> result)
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

        private static Component ComponentSelector(string label, Component cur, List<Component> components)
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

        private void GUIEndValueFloat()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueFloat = EditorGUILayout.FloatField(_src.endValueFloat);
            GUILayout.EndHorizontal();
        }

        private void GUIEndValueColor()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueColor = EditorGUILayout.ColorField(_src.endValueColor);
            GUILayout.EndHorizontal();
        }

        private void GUIEndValueV2()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueV3 = EditorGUILayout.Vector2Field("", _src.endValueV3, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        private void GUIEndValueV3()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueV3 = EditorGUILayout.Vector3Field("", _src.endValueV3, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        private void GUIEndValueZ()
        {
            GUILayout.BeginHorizontal();
            GUIToFromButton();
            _src.endValueV3.z = EditorGUILayout.FloatField("", _src.endValueV3.z, GUILayout.Height(16));
            GUILayout.EndHorizontal();
        }

        private void GUIToFromButton()
        {
            if (GUILayout.Button(_src.isFrom ? "FROM" : "TO", GUILayout.Width(90))) _src.isFrom = !_src.isFrom;
            GUILayout.Space(EditorGUIUtility.labelWidth - 90);
        }

        #endregion
    }
}