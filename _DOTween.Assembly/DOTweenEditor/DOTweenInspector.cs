using System.Text;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace DG.DOTweenEditor.UI
{
    public class DOTweenInspector : CustomOdinEditorWindow
    {
        private static readonly StringBuilder _sb = new();

        [MenuItem("Window/DOTween Inspector")]
        private static void Open()
        {
            GetWindow<DOTweenInspector>().Show();
        }

        protected override void Initialize()
        {
            base.Initialize();

            OnBeginGUI += () =>
            {
                GUILayout.Label(
                    "Pool State: " +
                    $"Tweeners={TweenPool.SumPooledTweeners()} " +
                    $"Sequences={TweenPool.SumPooledSequences()}");
            };
        }

        protected override void DrawEditors()
        {
            base.DrawEditors();

            if (EditorApplication.isPlaying is false)
                return;

            // Draw playing tweens.
            var tweens = TweenManager.Tweens.StartIterate();
            try
            {
                foreach (var t in tweens) DrawTweenButton(t);
            }
            catch (ExitGUIException)
            {
                // ExitGUIException is thrown when the user interacts with the GUI.
            }
            finally
            {
                TweenManager.Tweens.EndIterate();
            }
        }

        private static void DrawTweenButton(Tween tween, bool isSequenced = false)
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

        private static readonly GUIContent _gcPlay = new("►");
        private static readonly GUIContent _gcPause = new("❚❚");

        private static void DrawPlayToggle(Tween tween)
        {
            var isPlaying = tween.isPlaying;
            if (GUILayout.Button(isPlaying ? _gcPause : _gcPlay, GUILayout.Width(30)))
            {
                if (isPlaying) TweenManager.Pause(tween);
                else TweenManager.Play(tween);
            }
        }

        private static string BuildTweenLabel(Tween t)
        {
            Assert.AreEqual(0, _sb.Length, "StringBuilder not empty");
            if (t is Sequence)
                _sb.Append("[SEQUENCE] ");
            if (string.IsNullOrEmpty(t.debugHint) == false)
                _sb.Append(t.debugHint).Append(';');
            _sb.Append(t.id.Str()).Append(";");
            _sb.Append(t.target.SafeName());
            var str = _sb.ToString();
            _sb.Clear();
            return str;
        }
    }
}