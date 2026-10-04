using UnityEngine;

/// <summary>Very small on-screen text: the controls, a timer, and a message when the goal is reached.</summary>
public class LevelHud : MonoBehaviour
{
    float startTime;
    float finishTime = -1f;
    GUIStyle small, big;

    void Start() { startTime = Time.time; }

    public void ShowGoalReached()
    {
        if (finishTime < 0f) finishTime = Time.time - startTime;
    }

    void OnGUI()
    {
        if (small == null)
        {
            small = new GUIStyle(GUI.skin.label) { fontSize = 16 };
            big = new GUIStyle(GUI.skin.label) { fontSize = 44, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        }
        GUI.Label(new Rect(14, 10, 900, 30), "WASD / arrows: move   Shift: sprint   Space: jump   Mouse: look   Esc: free the mouse", small);
        float t = finishTime >= 0f ? finishTime : Time.time - startTime;
        GUI.Label(new Rect(14, 34, 400, 30), "Time: " + t.ToString("0.0") + " s", small);
        if (finishTime >= 0f)
            GUI.Label(new Rect(0, Screen.height * 0.35f, Screen.width, 80), "GOAL REACHED!  " + finishTime.ToString("0.0") + " s", big);
    }
}
