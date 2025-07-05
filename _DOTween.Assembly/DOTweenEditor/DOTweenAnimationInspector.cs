#nullable enable
using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.Utilities.Editor;
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
            { DOTweenAnimationType.None, new[] { typeof(Transform) } }, // placeholder.
            { DOTweenAnimationType.Move, new[] { typeof(Transform) } },
            { DOTweenAnimationType.Rotate, new[] { typeof(Transform) } },
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

        private DOTweenAnimation _src = null!;

        #region MonoBehaviour Methods

        private void OnEnable()
        {
            _src = (DOTweenAnimation) target;
        }

        public override void OnInspectorGUI()
        {
            GUIHelper.PushLabelWidth(50);

            // Preview in editor
            var previewId = _src.GetInstanceID();
            var wasPreviewing = DOTweenPreviewManager.IsPreviewing(previewId, out var previewingTween);
            if (Editing.Yes(_src))
            {
                if (wasPreviewing is false)
                {
                    if (GUILayout.Button("► Play"))
                        DOTweenPreviewManager.StartPreview(_src.CreateTweenInstance().SetId(previewId));
                }
                else
                {
                    if (GUILayout.Button("■ Stop"))
                        DOTweenPreviewManager.StopPreview(previewingTween);
                }
            }
            EditorGUILayout.Space(6);

            using var _ = new EditorGUI.DisabledScope(wasPreviewing); // disable if previewing
            EditorGUI.BeginChangeCheck();
            if (wasPreviewing is false)
                Undo.RecordObject(_src, "DOTween Animation");

            // Reset properties if the animation type changed.
            var prevType = _src.animationType;
            var type = _src.animationType = (DOTweenAnimationType) EditorGUILayout.EnumPopup("Type", _src.animationType);
            if (prevType != _src.animationType)
            {
                // Set default optional values based on animation type
                _src.endValueFloat = 0;
                _src.endValueV3 = default;
                _src.endValueColor = Color.white;
                _src.optionalBool0 = false;
                _src.optionalBool1 = false;
                _src.optionalFloat0 = 0;
                _src.optionalInt0 = 0;

                switch (type)
                {
                    case DOTweenAnimationType.Move:
                    case DOTweenAnimationType.Rotate:
                    case DOTweenAnimationType.Scale:
                        _src.optionalBool0 = type is DOTweenAnimationType.Scale;
                        break;
                    case DOTweenAnimationType.PunchPosition:
                    case DOTweenAnimationType.PunchRotation:
                    case DOTweenAnimationType.PunchScale:
                        _src.endValueV3 = type == DOTweenAnimationType.PunchRotation ? new Vector3(0, 180, 0) : Vector3.one;
                        _src.optionalFloat0 = 1;
                        _src.optionalInt0 = 10;
                        break;
                    case DOTweenAnimationType.ShakePosition:
                    case DOTweenAnimationType.ShakeRotation:
                    case DOTweenAnimationType.ShakeScale:
                        _src.endValueV3 = type is DOTweenAnimationType.ShakeRotation ? new Vector3(90, 90, 90) : Vector3.one;
                        _src.optionalInt0 = 10;
                        _src.optionalFloat0 = 90;
                        _src.optionalBool1 = true;
                        break;
                    case DOTweenAnimationType.UIAnchors:
                        _src.endValueV3 = new Vector3(0.5f, 0.5f, 0);
                        break;
                }
            }


            // Draw the target selector.
            _src.target = GUIComponentSelector("Target", type, _src.gameObject, _src.target);


            // Draw Duration & Delay.
            EditorGUILayout.BeginHorizontal();
            _src.duration = EditorGUILayout.FloatField("Duration", _src.duration);
            if (_src.duration < 0) _src.duration = 0;
            _src.delay = EditorGUILayout.FloatField("Delay", _src.delay);
            if (_src.delay < 0) _src.delay = 0;
            EditorGUILayout.EndHorizontal();


            // Draw the value, relative, from/to.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Value");
            // End value and eventual specific options
            switch (type)
            {
                case DOTweenAnimationType.None: // placeholder
                case DOTweenAnimationType.Move:
                    GUIValue_V2();
                    break;
                case DOTweenAnimationType.Rotate:
                    GUIValue_Z();
                    break;
                case DOTweenAnimationType.Scale:
                    if (_src.optionalBool0) GUIValue_Float();
                    else GUIValue_V2();
                    break;
                case DOTweenAnimationType.Color:
                    GUIValue_Color();
                    break;
                case DOTweenAnimationType.Fade:
                    GUIValue_Float();
                    if (_src.endValueFloat < 0) _src.endValueFloat = 0;
                    break;
                case DOTweenAnimationType.PunchPosition:
                case DOTweenAnimationType.PunchRotation:
                case DOTweenAnimationType.PunchScale:
                    if (type is DOTweenAnimationType.PunchRotation) GUIValue_Z();
                    else GUIValue_V2();
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("Vibrato"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("Elasticity"), _src.optionalFloat0, 0, 1);
                    break;
                case DOTweenAnimationType.ShakePosition:
                case DOTweenAnimationType.ShakeRotation:
                case DOTweenAnimationType.ShakeScale:
                    if (type is DOTweenAnimationType.ShakeRotation) GUIValue_Z();
                    else GUIValue_V2();
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("Vibrato"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("Randomness"), _src.optionalFloat0, 0, 90);
                    _src.optionalBool1 = EditorGUILayout.Toggle(new GUIContent("FadeOut"), _src.optionalBool1);
                    break;
                case DOTweenAnimationType.UIAnchors:
                    GUIValue_V2();
                    break;
            }

            GUILayout.Space(4);
            if (type is DOTweenAnimationType.Scale)
                _src.optionalBool0 = GUIPushToggle("Uni", _src.optionalBool0, width: 38);
            GUIFromTo(width: 38);
            if (type is DOTweenAnimationType.Move or DOTweenAnimationType.Rotate or DOTweenAnimationType.Scale)
                _src.isRelative = GUIPushToggle("Rel", _src.isRelative, width: 38);
            EditorGUILayout.EndHorizontal();


            // Ease
            GUILayout.BeginHorizontal();
            _src.easeType = (Ease) EditorGUILayout.EnumPopup("Ease", _src.easeType, GUILayout.MinWidth(60));
            if (_src.easeType == Ease.INTERNAL_Custom)
                _src.easeCurve = EditorGUILayout.CurveField(_src.easeCurve);
            GUILayout.EndHorizontal();


            // Loop
            GUILayout.BeginHorizontal();
            _src.loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), _src.loops);
            if (_src.loops < -1) _src.loops = -1;
            if (_src.loops is > 1 or -1)
                _src.loopType = (LoopType) EditorGUILayout.EnumPopup(_src.loopType);
            GUILayout.EndHorizontal();


            // Flags
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Flags");
            _src.autoGenerate = GUIPushToggle("Auto Gen", _src.autoGenerate);
            _src.autoPlay = GUIPushToggle("Auto Play", _src.autoPlay);
            _src.autoKill = GUIPushToggle("Auto Kill", _src.autoKill);
            GUILayout.EndHorizontal();


            GUIHelper.PopLabelWidth();

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_src);
        }

        #endregion

        #region Methods

        private static readonly List<Component> _compBuf = new();

        private static Component GUIComponentSelector(
            string label, DOTweenAnimationType animType, GameObject go, Component cur)
        {
            _compBuf.Clear();

            // collect targets
            var types = _eligibleTargetTypes[animType];
            foreach (var t in types)
            {
                if (go.TryGetComponent(t, out var targetComp))
                    _compBuf.Add(targetComp);
            }

            // only one component found, return it directly
            var count = _compBuf.Count;
            if (count is 1) return _compBuf[0];

            // multiple components found, show a popup
            var options = new string[count];
            for (var i = 0; i < count; i++)
                options[i] = _compBuf[i].GetType().Name;
            var index = Array.IndexOf(_compBuf.ToArray(), cur);
            if (index is -1) index = 0;
            var newIndex = EditorGUILayout.Popup(label, index, options);
            return _compBuf[newIndex];
        }

        private void GUIValue_Float() => _src.endValueFloat = EditorGUILayout.FloatField(_src.endValueFloat);
        private void GUIValue_Color() => _src.endValueColor = EditorGUILayout.ColorField(_src.endValueColor);
        private void GUIValue_V2() => _src.endValueV3 = EditorGUILayout.Vector2Field("", _src.endValueV3);
        private void GUIValue_Z() => _src.endValueV3.z = EditorGUILayout.FloatField(_src.endValueV3.z);

        private void GUIFromTo(float width)
        {
            var label = _src.isFrom ? "From" : "To";
            if (GUILayout.Button(label, GUILayout.Width(width)))
            {
                _src.isFrom = !_src.isFrom;
                GUI.changed = true;
            }
        }

        private static bool GUIPushToggle(string label, bool value, float width = 64)
        {
            GUIHelper.PushColor(value ? Color.green : Color.white);
            var changed = GUILayout.Button(label, GUILayout.Width(width));
            if (changed) GUI.changed = true;
            GUIHelper.PopColor();
            return changed ? !value : value;
        }

        #endregion
    }
}