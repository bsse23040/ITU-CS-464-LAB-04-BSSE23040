using UnityEngine;

/// <summary>
/// Walks the player along a list of waypoints by feeding the normal PlayerController, so it uses the real movement code.
/// Used only to record the walkthrough video: it stays off unless the Editor recorder asks for it (Temp/autopilot_on).
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class RouteAutopilot : MonoBehaviour
{
    public Vector3[] waypoints;
    public float arriveDistance = 0.35f;
    public float startDelay = 2f;
    public float endDelay = 3f;

    PlayerController player;
    int index;
    float timer;
    bool finished;

    void Awake()
    {
        player = GetComponent<PlayerController>();
#if UNITY_EDITOR
        enabled = System.IO.File.Exists("Temp/autopilot_on");
#else
        enabled = false;
#endif
    }

    void OnEnable() { timer = 0f; }

    void Update()
    {
        timer += Time.deltaTime;
        player.useExternalInput = true;
        if (finished || timer < startDelay || waypoints == null || index >= waypoints.Length)
        {
            player.externalWorldDirection = Vector3.zero;
            if (!finished && waypoints != null && index >= waypoints.Length) { finished = true; timer = 0f; Debug.Log("[Autopilot] route finished"); }
            if (finished && timer > endDelay) Finish();
            return;
        }

        Vector3 to = waypoints[index] - transform.position; to.y = 0f;
        if (to.magnitude < arriveDistance) { index++; return; }
        player.externalWorldDirection = to.normalized;
    }

    void Finish()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        enabled = false;
    }
}
