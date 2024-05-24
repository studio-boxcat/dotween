using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 1591
namespace DG.Tweening.Core
{
    [AddComponentMenu("")]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    class DOTweenUnityBridge : MonoBehaviour
    {
        public static bool IsPlaying()
        {
#if UNITY_EDITOR
            return _editor_IsPlaying;
#else
            return Application.isPlaying;
#endif
        }

        static GameObject _instance;

        void Update() => DOTween.Update();

        void OnDestroy()
        {
            Assert.IsTrue(ReferenceEquals(_instance, gameObject), "Instance mismatch");
            _instance = null;
#if UNITY_EDITOR
            DOTween.Editor_Clear();
#endif
        }

        internal static void Create()
        {
            Assert.IsTrue(Application.isPlaying, "Cannot create a DOTweenUnityBridge instance outside Play mode");
            Assert.IsNull(_instance, "An instance of DOTween is already running");
            _instance = new GameObject();
#if DEBUG
            _instance.name = nameof(DOTweenUnityBridge);
#endif
            DontDestroyOnLoad(_instance);
            _instance.AddComponent<DOTweenUnityBridge>();
        }

#if UNITY_EDITOR
        static bool _editor_IsPlaying;

        static DOTweenUnityBridge()
        {
            _editor_IsPlaying = false;

            UnityEditor.EditorApplication.playModeStateChanged += state =>
            {
                if (state is UnityEditor.PlayModeStateChange.ExitingPlayMode)
                {
                    Assert.IsTrue(_editor_IsPlaying, "DOTweenUnityBridge is not playing");
                    _editor_IsPlaying = false;
                }
                else if (state is UnityEditor.PlayModeStateChange.ExitingEditMode)
                {
                    Assert.IsFalse(_editor_IsPlaying, "DOTweenUnityBridge is playing");
                    _editor_IsPlaying = true;
                }
            };
        }
#endif
    }
}