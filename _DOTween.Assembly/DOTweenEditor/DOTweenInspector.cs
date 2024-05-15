using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace DG.DOTweenEditor.UI
{
    public class DOTweenInspector : OdinEditorWindow
    {
        static readonly StringBuilder _sb = new();

        [MenuItem("Window/DOTween Inspector")]
        static void Open()
        {
            GetWindow<DOTweenInspector>().Show();
        }

        protected override void OnImGUI()
        {
            if (EditorApplication.isPlaying is false)
                return;

            GUILayout.Label("Pooled tweens");
            GUILayout.Label("    Tweeners: " + TweenPool.SumPooledTweeners());
            GUILayout.Label("    Sequences: " + TweenPool.SumPooledSequences());

            base.OnImGUI();

            // Draw playing tweens.
            var tweens = TweenManager.Tweens.StartIterate();
            foreach (var t in tweens) DrawTweenButton(t);
            TweenManager.Tweens.EndIterate();
        }

        static void DrawTweenButton(Tween tween, bool isSequenced = false)
        {
            var label = BuildTweenLabel(tween);

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
                    GUILayout.Label(label);
                }

                if (!isSequenced)
                    GUILayout.EndHorizontal();
            }
            else if (tween is Sequence s)
            {
                if (!isSequenced)
                {
                    GUILayout.BeginHorizontal();
                    DrawPlayToggle(s);
                }

                GUILayout.Label(label);

                if (!isSequenced)
                    GUILayout.EndHorizontal();

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

        static string BuildTweenLabel(Tween t)
        {
            Assert.AreEqual(0, _sb.Length, "StringBuilder not empty");
            if (t is Sequence)
                _sb.Append("[SEQUENCE] ");
            if (string.IsNullOrEmpty(t.debugHint) == false)
                _sb.Append(t.debugHint).Append(';');
            if (t.id != Tween.invalidId)
                _sb.Append(t.id).Append(";");
            _sb.Append(t.target ?? "null");
            var str = _sb.ToString();
            _sb.Clear();
            return str;
        }
    }
}