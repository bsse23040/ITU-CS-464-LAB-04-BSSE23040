using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Camera that follows the player from behind and slightly above, smoothly.
/// The mouse orbits the camera around the player; the camera pulls in when a wall is between it and the player.
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 6f;
    public float height = 1.6f;              // height of the point the camera looks at, above the player's feet
    public float followSmoothing = 12f;
    public float mouseSensitivity = 0.12f;
    public Vector2 pitchLimits = new Vector2(-10f, 55f);
    public LayerMask obstructionMask = ~0;

    [Tooltip("Turn the camera to stay behind the player (used for the recorded walkthrough). Off for normal play.")]
    public bool autoAlignBehind;
    public float alignSpeed = 120f;

    float yaw;
    float pitch = 18f;

    void Start()
    {
        if (target != null) yaw = target.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;   // press Esc to get the mouse back
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        }

        if (autoAlignBehind) yaw = Mathf.MoveTowardsAngle(yaw, target.eulerAngles.y, alignSpeed * Time.deltaTime);
        else if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * mouseSensitivity;
            pitch = Mathf.Clamp(pitch - delta.y * mouseSensitivity, pitchLimits.x, pitchLimits.y);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = target.position + Vector3.up * height;
        Vector3 desired = focus - rotation * Vector3.forward * distance;

        RaycastHit hit;                                // keep the camera in front of walls
        if (Physics.Linecast(focus, desired, out hit, obstructionMask, QueryTriggerInteraction.Ignore) && !hit.transform.IsChildOf(target))
            desired = hit.point + hit.normal * 0.3f;

        float t = 1f - Mathf.Exp(-followSmoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desired, t);
        transform.rotation = Quaternion.LookRotation(focus - transform.position, Vector3.up);
    }
}
