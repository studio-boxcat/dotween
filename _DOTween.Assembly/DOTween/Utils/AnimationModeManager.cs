#if UNITY_EDITOR
#nullable enable

using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public static class AnimationModeManager
{
    private static int _counter;
    private static bool _willStop;

    public static void Start()
    {
        if (_willStop)
        {
            L.I("[AnimationModeManager] Canceling AnimationMode stop: counter=" + _counter);
            Assert.IsTrue(_counter is 0, "[AnimationModeManager] _willStop should only be true when _counter is 0.");
            _willStop = false;
            _counter++;
            return;
        }

        if (_counter++ is 0)
        {
            L.I("[AnimationModeManager] Starting AnimationMode");
            AnimationMode.StartAnimationMode(); // for screen refresh.
            EditorSettings.prefabModeAllowAutoSave = false;
        }
        else
        {
            L.I("[AnimationModeManager] AnimationMode already started, incrementing counter: " + _counter);
        }
    }

    public static void Stop()
    {
        if (--_counter is not 0)
        {
            L.I("[AnimationModeManager] AnimationMode not stopped, counter=" + _counter);
            return;
        }

        // delay to prevent asset saving while frequently changing the AnimationMode state.
        // e.g. Stop and Start in the same frame.
        Assert.IsFalse(_willStop, "[AnimationModeManager] _willStop should be false when counter is 0.");
        _willStop = true;
        EditorApplication.delayCall += () =>
        {
            if (_willStop is false)
            {
                L.I("[AnimationModeManager] AnimationMode already stopped, counter=" + _counter);
                return;
            }

            L.I("[AnimationModeManager] Stopping AnimationMode");
            _willStop = false;
            AnimationMode.StopAnimationMode(); // for screen refresh.
            EditorSettings.prefabModeAllowAutoSave = true;

            ForceRefreshScene();
        };
    }

    public static void ForceRefreshScene()
    {
        // XXX: force refresh the Scene (or PrefabStage).
        // Canvas.ForceUpdateCanvases(), InternalEditorUtility.RepaintAllViews() or EditorApplication.QueuePlayerLoopUpdate() does not work.
        var cr = Resources.FindObjectsOfTypeAll<CanvasRenderer>().FirstOrDefault();
        if (cr) EditorUtility.SetDirty(cr);
    }
}
#endif