#nullable enable
using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
    [CustomEditor(typeof(DOTweenAnim))]
    public class DOTweenAnimInspector : Editor
    {
        private DOTweenAnim _src = null!;
        private SerializedProperty _durationProp = null!;
        private SerializedProperty _delayProp = null!;
        private SerializedProperty _endValueProp = null!;

        private void OnEnable()
        {
            _src = (DOTweenAnim) target;
            _durationProp = serializedObject.FindProperty("duration");
            _delayProp = serializedObject.FindProperty("delay");
            _endValueProp = serializedObject.FindProperty("endValue");
        }

        public override void OnInspectorGUI()
        {
            GUIHelper.PushLabelWidth(50);

            // Preview in editor
            var previewId = _src.GetInstanceID();
            var wasPreviewing = DOTweenPreviewManager.IsPreviewing(previewId, out var previewingTween);
            if (Editing.Yes(_src) && _src.NoComponent<DOTweenGroup>())
            {
                if (wasPreviewing is false)
                {
                    if (GUILayout.Button("► Play"))
                        DOTweenPreviewManager.StartPreview(_src.CreateTween(play: true).SetId(previewId));
                }
                else
                {
                    if (GUILayout.Button("■ Stop"))
                        DOTweenPreviewManager.StopPreview(previewingTween);
                }

                EditorGUILayout.Space(6);
            }

            using var _ = new EditorGUI.DisabledScope(wasPreviewing); // disable if previewing
            EditorGUI.BeginChangeCheck();
            if (wasPreviewing is false)
                Undo.RecordObject(_src, "DOTween Animation");

            // Reset properties if the animation type changed.
            var prevType = _src.animType;
            var type = _src.animType = (DOTweenAnimType) EditorGUILayout.EnumPopup("Type", _src.animType);
            if (prevType != _src.animType)
            {
                // Set default optional values based on animation type
                _src.endValue = default;
                _src.optionalFloat = 0;
                _src.optionalInt = 0;
                _src.uniformScale = false;

                // set default value.
                switch (type)
                {
                    case DOTweenAnimType.Scale:
                        _src.endValue = _src.isRelative ? default : Vector3.one;
                        _src.uniformScale = true; // uniform scale
                        break;
                    case DOTweenAnimType.PunchPos:
                    case DOTweenAnimType.PunchRot:
                    case DOTweenAnimType.PunchScale:
                        _src.endValue = type == DOTweenAnimType.PunchRot ? new Vector3(0, 180, 0) : Vector3.one;
                        _src.optionalFloat = 1;
                        _src.optionalInt = 10;
                        break;
                    case DOTweenAnimType.ShakePos:
                    case DOTweenAnimType.ShakeRot:
                    case DOTweenAnimType.ShakeScale:
                        _src.endValue = type is DOTweenAnimType.ShakeRot ? new Vector3(90, 90, 90) : Vector3.one;
                        _src.optionalInt = 10;
                        _src.optionalFloat = 90;
                        break;
                    case DOTweenAnimType.AnchorPos:
                        _src.endValue = new Vector3(0, 0, 0);
                        break;
                    case DOTweenAnimType.Anchor:
                        _src.endValue = new Vector3(0.5f, 0.5f, 0);
                        break;
                }

                // set default relative value
                _src.isRelative = CanBeRelative(type);
            }


            // Draw the target selector.
            _src.target = GUI_ComponentSelector("Target", type, _src.gameObject, _src.target);


            // Draw Duration & Delay.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_durationProp);
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(_delayProp);
            EditorGUILayout.EndHorizontal();


            // Draw the value, relative, from/to.
            EditorGUILayout.BeginHorizontal();
            GUI_Value(GetValueType(type, _src.uniformScale));

            // type specific options
            switch (type)
            {
                case DOTweenAnimType.Scale:
                    _src.uniformScale = GUI_PushToggle("Uni", _src.uniformScale, width: 38); // uniform scale
                    break;
                case DOTweenAnimType.Fade:
                    if (_src.endValue.x < 0) _src.endValue.x = 0; // lower bound 0
                    break;
                case DOTweenAnimType.PunchPos:
                case DOTweenAnimType.PunchRot:
                case DOTweenAnimType.PunchScale:
                    _src.optionalInt = EditorGUILayout.IntSlider(new GUIContent("V", "Vibrato"), _src.optionalInt, 1, 50);
                    _src.optionalFloat = EditorGUILayout.Slider(new GUIContent("E", "Elasticity"), _src.optionalFloat, 0, 1);
                    break;
                case DOTweenAnimType.ShakePos:
                case DOTweenAnimType.ShakeRot:
                case DOTweenAnimType.ShakeScale:
                    _src.optionalInt = EditorGUILayout.IntSlider(new GUIContent("V", "Vibrato"), _src.optionalInt, 1, 50);
                    _src.optionalFloat = EditorGUILayout.Slider(new GUIContent("R", "Randomness"), _src.optionalFloat, 0, 90);
                    break;
            }

            GUILayout.Space(4);
            GUI_FromTo(width: 38);
            if (CanBeRelative(type))
                _src.isRelative = GUI_PushToggle("Rel", _src.isRelative, width: 38);
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
            _src.autoGenerate = GUI_PushToggle("Auto Gen", _src.autoGenerate);
            _src.autoPlay = GUI_PushToggle("Auto Play", _src.autoPlay);
            _src.autoKill = GUI_PushToggle("Auto Kill", _src.autoKill);
            GUILayout.EndHorizontal();


            GUIHelper.PopLabelWidth();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_src);
            }
        }

        private static bool CanBeRelative(DOTweenAnimType type)
        {
            return type
                is DOTweenAnimType.Move or DOTweenAnimType.MoveY
                or DOTweenAnimType.Rotate or DOTweenAnimType.Scale;
        }

        private static ValueType GetValueType(DOTweenAnimType type, bool uniformScale)
        {
            return type switch
            {
                DOTweenAnimType.None => ValueType.Float, // placeholder
                DOTweenAnimType.Move => ValueType.XY,
                DOTweenAnimType.MoveY => ValueType.Y,
                DOTweenAnimType.Scale => uniformScale ? ValueType.Uniform : ValueType.XY,
                DOTweenAnimType.Fade => ValueType.Float,
                DOTweenAnimType.PunchPos or DOTweenAnimType.PunchScale
                    or DOTweenAnimType.ShakePos or DOTweenAnimType.ShakeScale
                    => ValueType.XY,
                DOTweenAnimType.Rotate or DOTweenAnimType.PunchRot or DOTweenAnimType.ShakeRot
                    => ValueType.Z,
                DOTweenAnimType.Anchor => ValueType.XY,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static readonly List<Component> _targetBuf = new();

        private static Component GUI_ComponentSelector(
            string label, DOTweenAnimType animType, GameObject go, Component curTarget)
        {
            _targetBuf.Clear();

            // collect targets
            var types = DOTweenAnim.GetEligibleTargetTypes(animType);
            foreach (var t in types)
            {
                if (go.TryGetComponent(t, out var targetComp))
                    _targetBuf.Add(targetComp);
            }

            // only one component found, return it directly
            var count = _targetBuf.Count;
            if (count is 1 && curTarget.RefEq(_targetBuf[0]))
                return _targetBuf[0];

            // multiple components found, show a popup
            var options = new string[count];
            for (var i = 0; i < count; i++)
                options[i] = _targetBuf[i].GetType().Name;
            var index = Array.IndexOf(_targetBuf.ToArray(), curTarget);
            if (index is -1) index = 0;
            var newIndex = EditorGUILayout.Popup(label, index, options);
            return _targetBuf[newIndex];
        }

        private void GUI_Value(ValueType valueType)
        {
            var r = EditorGUILayout.GetControlRect();
            var label = EditorGUI.BeginProperty(r, new GUIContent("Value"), _endValueProp);
            r = EditorGUI.PrefixLabel(r, label);

            ref var v = ref _src.endValue;
            switch (valueType)
            {
                case ValueType.Float:
                case ValueType.Uniform:
                    v.x = EditorGUI.FloatField(r, v.x);
                    break;
                case ValueType.XY:
                    v.AssignXY(EditorGUI.Vector2Field(r, GUIContent.none, v));
                    break;
                case ValueType.Y:
                    v.y = EditorGUI.FloatField(r, v.y);
                    break;
                case ValueType.Z:
                    v.z = EditorGUI.FloatField(r, v.z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
            }

            if (valueType is ValueType.Uniform)
                v = new Vector3(v.x, v.x, v.z); // uniform scale, keep z as is.

            EditorGUI.EndProperty();
        }

        private void GUI_FromTo(float width)
        {
            var label = _src.isFrom ? "From" : "To";
            if (GUILayout.Button(label, GUILayout.Width(width)))
            {
                _src.isFrom = !_src.isFrom;
                GUI.changed = true;
            }
        }

        private static bool GUI_PushToggle(string label, bool value, float width = 64)
        {
            GUIHelper.PushColor(value ? Color.green : Color.white);
            var changed = GUILayout.Button(label, GUILayout.Width(width));
            if (changed) GUI.changed = true;
            GUIHelper.PopColor();
            return changed ? !value : value;
        }

        private enum ValueType
        {
            Float,
            XY,
            Y,
            Z,
            Uniform,
        }
    }
}