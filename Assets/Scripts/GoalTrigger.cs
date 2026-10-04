using UnityEngine;

/// <summary>Put on a trigger collider at the goal. Tells the HUD when the player gets there.</summary>
[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    void Reset() { GetComponent<Collider>().isTrigger = true; }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        var hud = FindFirstObjectByType<LevelHud>();
        if (hud != null) hud.ShowGoalReached();
    }
}
