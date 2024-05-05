// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/03/12 16:03

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using DOTweenSettings = DG.Tweening.Core.DOTweenSettings;
#if true // UI_MARKER
using UnityEngine.UI;
#endif

namespace DG.DOTweenEditor
{
    [CustomEditor(typeof(DOTweenAnimation))]
    public class DOTweenAnimationInspector : Editor
    {
        enum FadeTargetType
        {
            CanvasGroup,
            Graphic,
        }

        enum ChooseTargetMode
        {
            None,
            BetweenCanvasGroupAndImage
        }

        static readonly Dictionary<DOTweenAnimation.AnimationType, Type[]> _AnimationTypeToComponent = new() {
            { DOTweenAnimation.AnimationType.Move, new[] { typeof(RectTransform), typeof(Transform) }},
            { DOTweenAnimation.AnimationType.Rotate, new[] { typeof(Transform) }},
            { DOTweenAnimation.AnimationType.LocalMove, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.LocalRotate, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.Color, new[] { typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer), }},
            { DOTweenAnimation.AnimationType.Fade, new[] { typeof(CanvasGroup), typeof(Graphic), typeof(SpriteRenderer), typeof(Renderer) }},
            { DOTweenAnimation.AnimationType.PunchPosition, new[] { typeof(RectTransform), typeof(Transform) }},
            { DOTweenAnimation.AnimationType.PunchRotation, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakePosition, new[] { typeof(RectTransform), typeof(Transform) }},
            { DOTweenAnimation.AnimationType.ShakeRotation, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.UIAnchors, new[] { typeof(RectTransform) } },
        };

        DOTweenAnimation _src;
        bool _runtimeEditMode; // If TRUE allows to change and save stuff at runtime
        bool _refreshRequired; // If TRUE refreshes components data
        int _totComponentsOnSrc; // Used to determine if a Component is added or removed from the source
        bool _isLightSrc; // Used to determine if we're tweening a Light, to set the max Fade value to more than 1
#pragma warning disable 414
        ChooseTargetMode _chooseTargetMode = ChooseTargetMode.None;
#pragma warning restore 414

        #region MonoBehaviour Methods

        void OnEnable()
        {
            _src = target as DOTweenAnimation;
        }

        void OnDisable()
        {
            DOTweenPreviewManager.StopAllPreviews();
        }

        public override void OnInspectorGUI()
        {
            bool playMode = Application.isPlaying;
            _runtimeEditMode = _runtimeEditMode && playMode;

            if (playMode) {
                if (_runtimeEditMode) {
                    
                } else {
                    GUILayout.Space(8);
                    GUILayout.Label("Animation Editor disabled while in play mode");
                    if (GUILayout.Button(new GUIContent("Activate Edit Mode", "Switches to Runtime Edit Mode, where you can change animations values and restart them"))) {
                        _runtimeEditMode = true;
                    }
                    GUILayout.Label("NOTE: when using DOPlayNext, the sequence is determined by the DOTweenAnimation Components order in the target GameObject's Inspector");
                    GUILayout.Space(10);
                    if (!_runtimeEditMode) return;
                }
            }

            Undo.RecordObject(_src, "DOTween Animation");

            EditorGUIUtility.labelWidth = 100;

            if (playMode) {
                // GUILayout.Space(4);
                // DeGUILayout.Toolbar("Edit Mode Commands");
                // DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("TogglePause")) _src.tween.TogglePause();
                    if (GUILayout.Button("Rewind")) _src.tween.Rewind();
                    if (GUILayout.Button("Restart")) _src.tween.Restart();
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Commit changes and restart")) {
                        _src.tween.Rewind();
                        _src.tween.Kill();
                        if (_src.isValid) {
                            _src.CreateTween();
                            _src.tween.Play();
                        }
                    }
                //     GUILayout.Label("To apply your changes when exiting Play mode, use the Component's upper right menu and choose \"Copy Component\", then \"Paste Component Values\" after exiting Play mode", DeGUI.styles.label.wordwrap);
                // DeGUILayout.EndVBox();
            } else {
                GUILayout.BeginHorizontal();
                bool hasManager = _src.GetComponent<DOTweenVisualManager_Custom>() != null;
                if (!hasManager) {
                    if (GUILayout.Button(new GUIContent("Add Manager", "Adds a manager component which allows you to choose additional options for this gameObject"))) {
                        _src.gameObject.AddComponent<DOTweenVisualManager_Custom>();
                    }
                }
                GUILayout.EndHorizontal();
            }

            // Preview in editor
            bool isPreviewing = DOTweenPreviewManager.PreviewGUI(_src);
            EditorGUILayout.Space(6);

            EditorGUI.BeginDisabledGroup(isPreviewing);

            GameObject targetGO = _src.gameObject;

            if (targetGO == null) {
                // Uses external target gameObject but it's not set
                if (_src.target != null) {
                    _src.target = null;
                    GUI.changed = true;
                }
            } else {
                DOTweenAnimation.AnimationType prevAnimType = _src.animationType;
                _src.animationType = (DOTweenAnimation.AnimationType) EditorGUILayout.EnumPopup("Animation Type",_src.animationType);
                if (prevAnimType != _src.animationType) {
                    // Set default optional values based on animation type
                    switch (_src.animationType) {
                    case DOTweenAnimation.AnimationType.Move:
                    case DOTweenAnimation.AnimationType.LocalMove:
                    case DOTweenAnimation.AnimationType.Rotate:
                    case DOTweenAnimation.AnimationType.LocalRotate:
                    case DOTweenAnimation.AnimationType.Scale:
                        _src.endValueV3 = Vector3.zero;
                        _src.endValueFloat = 0;
                        _src.optionalBool0 = _src.animationType == DOTweenAnimation.AnimationType.Scale;
                        break;
                    case DOTweenAnimation.AnimationType.Color:
                    case DOTweenAnimation.AnimationType.Fade:
                        _isLightSrc = targetGO.GetComponent<Light>() != null;
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

                if (_src.animationType == DOTweenAnimation.AnimationType.None) {
                    _src.isValid = false;
                    if (GUI.changed) EditorUtility.SetDirty(_src);
                    return;
                }

                if (_refreshRequired || prevAnimType != _src.animationType || ComponentsChanged()) {
                    _refreshRequired = false;
                    _src.isValid = Validate(targetGO);
                    // See if we need to choose between multiple targets
#if true // UI_MARKER
                    if (_src.animationType == DOTweenAnimation.AnimationType.Fade && targetGO.GetComponent<CanvasGroup>() != null && targetGO.GetComponent<Image>() != null) {
                        _chooseTargetMode = ChooseTargetMode.BetweenCanvasGroupAndImage;
                        // Reassign target and forcedTargetType if lost
                        if (_src.forcedTargetType == DOTweenAnimation.TargetType.Unset) _src.forcedTargetType = _src.targetType;
                        switch (_src.forcedTargetType) {
                        case DOTweenAnimation.TargetType.CanvasGroup:
                            _src.target = targetGO.GetComponent<CanvasGroup>();
                            break;
                        case DOTweenAnimation.TargetType.Graphic:
                            _src.target = targetGO.GetComponent<Graphic>();
                            break;
                        }
                    } else {
#endif
                        _chooseTargetMode = ChooseTargetMode.None;
                        _src.forcedTargetType = DOTweenAnimation.TargetType.Unset;
#if true // UI_MARKER
                    }
#endif
                }

                if (!_src.isValid) {
                    GUI.color = Color.red;
                    GUILayout.BeginVertical(GUI.skin.box);
                    GUILayout.Label("No valid Component was found for the selected animation");
                    GUILayout.EndVertical();
                    GUI.color = Color.white;
                    if (GUI.changed) EditorUtility.SetDirty(_src);
                    return;
                }

#if true // UI_MARKER
                // Special cases in which multiple target types could be used (set after validation)
                if (_chooseTargetMode == ChooseTargetMode.BetweenCanvasGroupAndImage && _src.forcedTargetType != DOTweenAnimation.TargetType.Unset) {
                    FadeTargetType fadeTargetType = (FadeTargetType)Enum.Parse(typeof(FadeTargetType), _src.forcedTargetType.ToString());
                    DOTweenAnimation.TargetType prevTargetType = _src.forcedTargetType;
                    _src.forcedTargetType = (DOTweenAnimation.TargetType)Enum.Parse(typeof(DOTweenAnimation.TargetType), EditorGUILayout.EnumPopup(_src.animationType + " Target", fadeTargetType).ToString());
                    if (_src.forcedTargetType != prevTargetType) {
                        // Target type change > assign correct target
                        switch (_src.forcedTargetType) {
                        case DOTweenAnimation.TargetType.CanvasGroup:
                            _src.target = targetGO.GetComponent<CanvasGroup>();
                            break;
                        case DOTweenAnimation.TargetType.Graphic:
                            _src.target = targetGO.GetComponent<Graphic>();
                            break;
                        }
                    }
                }
#endif

                EditorGUILayout.BeginHorizontal();
                _src.duration = EditorGUILayout.FloatField("Duration", _src.duration);
                if (_src.duration < 0) _src.duration = 0;
                EditorGUILayout.Space(18, false);
                _src.delay = EditorGUILayout.FloatField("Delay", _src.delay);
                if (_src.delay < 0) _src.delay = 0;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(6);


                bool canBeRelative = true;
                // End value and eventual specific options
                switch (_src.animationType) {
                case DOTweenAnimation.AnimationType.Move:
                case DOTweenAnimation.AnimationType.LocalMove:
                    GUIEndValueV3();
                    _src.optionalBool0 = EditorGUILayout.Toggle("    Snapping", _src.optionalBool0);
                    canBeRelative = true;
                    break;
                case DOTweenAnimation.AnimationType.Rotate:
                case DOTweenAnimation.AnimationType.LocalRotate:
                    {
                        GUIEndValueV3();
                        _src.optionalRotationMode = (RotateMode)EditorGUILayout.EnumPopup("    Rotation Mode", _src.optionalRotationMode);
                    }
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
                    if (!_isLightSrc && _src.endValueFloat > 1) _src.endValueFloat = 1;
                    canBeRelative = false;
                    break;
                case DOTweenAnimation.AnimationType.PunchPosition:
                case DOTweenAnimation.AnimationType.PunchRotation:
                case DOTweenAnimation.AnimationType.PunchScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the punch vibrate"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Elasticity", "How much the vector will go beyond the starting position when bouncing backwards"), _src.optionalFloat0, 0, 1);
                    if (_src.animationType == DOTweenAnimation.AnimationType.PunchPosition) _src.optionalBool0 = EditorGUILayout.Toggle("    Snapping", _src.optionalBool0);
                    break;
                case DOTweenAnimation.AnimationType.ShakePosition:
                case DOTweenAnimation.AnimationType.ShakeRotation:
                case DOTweenAnimation.AnimationType.ShakeScale:
                    GUIEndValueV3();
                    canBeRelative = false;
                    _src.optionalInt0 = EditorGUILayout.IntSlider(new GUIContent("    Vibrato", "How much will the shake vibrate"), _src.optionalInt0, 1, 50);
                    _src.optionalFloat0 = EditorGUILayout.Slider(new GUIContent("    Randomness", "The shake randomness"), _src.optionalFloat0, 0, 90);
                    _src.optionalBool1 = EditorGUILayout.Toggle(new GUIContent("    FadeOut", "If selected the shake will fade out, otherwise it will constantly play with full force"), _src.optionalBool1);
                    if (_src.animationType == DOTweenAnimation.AnimationType.ShakePosition) _src.optionalBool0 = EditorGUILayout.Toggle("    Snapping", _src.optionalBool0);
                    break;
                case DOTweenAnimation.AnimationType.UIAnchors:
                    GUIEndValueV2();
                    canBeRelative = false;
                    break;
                }

                // Final settings
                if (canBeRelative) _src.isRelative = EditorGUILayout.Toggle("    Relative", _src.isRelative);
            }

            _src.easeType = (Ease) EditorGUILayout.EnumPopup("Ease", _src.easeType);
            if (_src.easeType == Ease.INTERNAL_Custom) {
                _src.easeCurve = EditorGUILayout.CurveField("   Ease Curve", _src.easeCurve);
            }
            _src.loops = EditorGUILayout.IntField(new GUIContent("Loops", "Set to -1 for infinite loops"), _src.loops);
            if (_src.loops < -1) _src.loops = -1;
            if (_src.loops > 1 || _src.loops == -1)
                _src.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", _src.loopType);

            EditorGUI.EndDisabledGroup();
            GUILayout.Space(6);


            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 70;
            _src.autoGenerate = EditorGUILayout.ToggleLeft(new GUIContent("AutoGenerate", "If selected, the tween will be generated at startup (during Start for RectTransform position tween, Awake for all the others)"), _src.autoGenerate);
            if (_src.autoGenerate) {
                _src.autoPlay = EditorGUILayout.ToggleLeft(new GUIContent("AutoPlay", "If selected, the tween will play automatically"), _src.autoPlay);
            }
            _src.autoKill = EditorGUILayout.ToggleLeft(new GUIContent("AutoKill", "If selected, the tween will be killed when it completes, and won't be reusable"), _src.autoKill);
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 110;
            GUILayout.EndHorizontal();


            if (GUI.changed) EditorUtility.SetDirty(_src);
        }

        #endregion

        #region Methods

        static readonly List<Component> _componentsBuf =  new();

        // Returns TRUE if the Component layout on the src gameObject changed (a Component was added or removed)
        bool ComponentsChanged()
        {
            int prevTotComponentsOnSrc = _totComponentsOnSrc;
            _src.gameObject.GetComponents(_componentsBuf);
            _totComponentsOnSrc = _componentsBuf.Count;
            return prevTotComponentsOnSrc != _totComponentsOnSrc;
        }

        // Checks if a Component that can be animated with the given animationType is attached to the src
        bool Validate(GameObject targetGO)
        {
            if (_src.animationType == DOTweenAnimation.AnimationType.None) return false;

            Component srcTarget;
            // First check for external plugins
            // Then check for regular stuff
            if (_AnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _AnimationTypeToComponent[_src.animationType]) {
                    srcTarget = targetGO.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = DOTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
            return false;
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

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    [InitializeOnLoad]
    static class Initializer
    {
        static Initializer()
        {
            DOTweenAnimation.OnReset += OnReset;
        }

        static void OnReset(DOTweenAnimation src)
        {
            DOTweenSettings settings = DOTweenSettings.Instance;
            if (settings == null) return;

            Undo.RecordObject(src, "DOTweenAnimation");
            src.autoPlay = settings.defaultAutoPlay == AutoPlay.All || settings.defaultAutoPlay == AutoPlay.AutoPlayTweeners;
            src.autoKill = settings.defaultAutoKill;
            EditorUtility.SetDirty(src);
        }
    }
}
