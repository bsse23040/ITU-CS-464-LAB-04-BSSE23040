using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Helper for recording the walkthrough: create the file Temp/play_now (and Temp/autopilot_on) and the editor opens the
/// player scene, enters Play mode, maximises the Game view and lets RouteAutopilot walk the default route.
/// </summary>
[InitializeOnLoad]
public static class Lab04Recorder
{
    const string Scene = "Assets/Scenes/Lab04_PlayerLevel.unity";

    static Lab04Recorder()
    {
        EditorApplication.update += Tick;
        EditorApplication.playModeStateChanged += OnState;
    }

    static void Tick()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!File.Exists("Temp/play_now")) return;
        File.Delete("Temp/play_now");
        EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
        Debug.Log("[Lab04Recorder] PLAY_START");
    }

    static void OnState(PlayModeStateChange state)
    {
        var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        if (state == PlayModeStateChange.EnteredPlayMode && File.Exists("Temp/autopilot_on"))
        {
            EditorApplication.delayCall += () =>
            {
                var w = EditorWindow.GetWindow(gameViewType);
                w.Focus(); w.maximized = true;
            };
        }
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if (File.Exists("Temp/autopilot_on"))
            {
                File.Delete("Temp/autopilot_on");
                Debug.Log("[Lab04Recorder] PLAY_END");
            }
            var w = EditorWindow.GetWindow(gameViewType);
            if (w != null && w.maximized) w.maximized = false;
        }
    }
}
