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
    [CustomEditor(typeof(DOTweenAnim))]
    public class DOTweenAnimInspector : Editor
    {
        private DOTweenAnim _src = null!;

        private void OnEnable()
        {
            _src = (DOTweenAnim) target;
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
            var prevType = _src.animationType;
            var type = _src.animationType = (DOTweenAnimType) EditorGUILayout.EnumPopup("Type", _src.animationType);
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
                    case DOTweenAnimType.Scale:
                        _src.optionalBool0 = true; // uniform scale
                        break;
                    case DOTweenAnimType.PunchPos:
                    case DOTweenAnimType.PunchRot:
                    case DOTweenAnimType.PunchScale:
                        _src.endValueV3 = type == DOTweenAnimType.PunchRot ? new Vector3(0, 180, 0) : Vector3.one;
                        _src.optionalFloat0 = 1;
                        _src.optionalInt0 = 10;
                        break;
                    case DOTweenAnimType.ShakePos:
                    case DOTweenAnimType.ShakeRot:
                    case DOTweenAnimType.ShakeScale:
                        _src.endValueV3 = type is DOTweenAnimType.ShakeRot ? new Vector3(90, 90, 90) : Vector3.one;
                        _src.optionalInt0 = 10;
                        _src.optionalFloat0 = 90;
                        _src.optionalBool1 = true;
                        break;
                    case DOTweenAnimType.UIAnchors:
                        _src.endValueV3 = new Vector3(0.5f, 0.5f, 0);
                        break;
                }

                // set default relative value
                _src.isRelative = CanBeRelative(type);
            }


            // Draw the target selector.
            _src.target = GUI_ComponentSelector("Target", type, _src.gameObject, _src.target);


            // Draw Duration & Delay.
            EditorGUILayout.BeginHorizontal();
            _src.duration = EditorGUILayout.FloatField("Duration", _src.duration);
            if (_src.duration < 0) _src.duration = 0;
            _src.delay = EditorGUILayout.FloatField("Delay", _src.delay);
            if (_src.delay < 0) _src.delay = 0;
            EditorGUILayout.EndHorizontal();


            // Draw the value, relative, from/to.
            EditorGUILayout.BeginHorizontal();
            GUI_Value(GetValueType(type, _src.optionalBool0));

            // type specific options
            switch (type)
            {
                case DOTweenAnimType.Scale:
                    _src.optionalBool0 = GUI_PushToggle("Uni", _src.optionalBool0, width: 38); // uniform scale
                    break;
                case DOTweenAnimType.Fade:
                    if (_src.endValueFloat < 0) _src.endValueFloat = 0;
                    break;
                case DOTweenAnimType.PunchPos:
                case DOTweenAnimType.PunchRot:
                case DOTweenAnimType.PunchScale:
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("V", "Vibrato"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("E", "Elasticity"), _src.optionalFloat0, 0, 1);
                    break;
                case DOTweenAnimType.ShakePos:
                case DOTweenAnimType.ShakeRot:
                case DOTweenAnimType.ShakeScale:
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("V", "Vibrato"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("R", "Randomness"), _src.optionalFloat0, 0, 90);
                    _src.optionalBool1 = EditorGUILayout.Toggle(new GUIContent("F", "FadeOut"), _src.optionalBool1);
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
                EditorUtility.SetDirty(_src);
        }

        private static bool CanBeRelative(DOTweenAnimType type)
        {
            return type
                is DOTweenAnimType.MoveY or DOTweenAnimType.Move
                or DOTweenAnimType.Rotate or DOTweenAnimType.Scale;
        }

        private static ValueType GetValueType(DOTweenAnimType type, bool optionalBool0)
        {
            return type switch
            {
                DOTweenAnimType.None => ValueType.Float, // placeholder
                DOTweenAnimType.MoveY => ValueType.Y,
                DOTweenAnimType.Move => ValueType.XY,
                DOTweenAnimType.Scale => optionalBool0 ? ValueType.Float : ValueType.XY, // uniform scale
                DOTweenAnimType.Color => ValueType.Color,
                DOTweenAnimType.Fade => ValueType.Float,
                DOTweenAnimType.PunchPos or DOTweenAnimType.PunchScale
                    or DOTweenAnimType.ShakePos or DOTweenAnimType.ShakeScale
                    => ValueType.XY,
                DOTweenAnimType.Rotate or DOTweenAnimType.PunchRot or DOTweenAnimType.ShakeRot
                    => ValueType.Z,
                DOTweenAnimType.UIAnchors => ValueType.XY,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static readonly Dictionary<DOTweenAnimType, Type[]> _eligibleTargetTypes = new()
        {
            { DOTweenAnimType.None, new[] { typeof(Transform) } }, // placeholder.
            { DOTweenAnimType.MoveY, new[] { typeof(Transform) } },
            { DOTweenAnimType.Move, new[] { typeof(Transform) } },
            { DOTweenAnimType.Rotate, new[] { typeof(Transform) } },
            { DOTweenAnimType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimType.Color, new[] { typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer), } },
            { DOTweenAnimType.Fade, new[] { typeof(CanvasGroup), typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer) } },
            { DOTweenAnimType.PunchPos, new[] { typeof(Transform) } },
            { DOTweenAnimType.PunchRot, new[] { typeof(Transform) } },
            { DOTweenAnimType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimType.ShakePos, new[] { typeof(Transform) } },
            { DOTweenAnimType.ShakeRot, new[] { typeof(Transform) } },
            { DOTweenAnimType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimType.UIAnchors, new[] { typeof(RectTransform) } },
        };

        private static readonly List<Component> _targetBuf = new();

        private static Component GUI_ComponentSelector(
            string label, DOTweenAnimType animType, GameObject go, Component curTarget)
        {
            _targetBuf.Clear();

            // collect targets
            var types = _eligibleTargetTypes[animType];
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
            EditorGUILayout.PrefixLabel("Value");
            switch (valueType)
            {
                case ValueType.Float:
                    _src.endValueFloat = EditorGUILayout.FloatField(_src.endValueFloat);
                    break;
                case ValueType.Color:
                    _src.endValueColor = EditorGUILayout.ColorField(_src.endValueColor);
                    break;
                case ValueType.XY:
                    _src.endValueV3.AssignXY(EditorGUILayout.Vector2Field("", _src.endValueV3));
                    break;
                case ValueType.Y:
                    _src.endValueV3.y = EditorGUILayout.FloatField(_src.endValueV3.y);
                    break;
                case ValueType.Z:
                    _src.endValueV3.z = EditorGUILayout.FloatField(_src.endValueV3.z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
            }
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
            Color,
            XY,
            Y,
            Z,
        }
    }
}