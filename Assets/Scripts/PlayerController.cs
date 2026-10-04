using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Third-person player movement for the Warehouse level.
/// WASD / arrow keys (or the left stick) move relative to the camera, Left Shift sprints, Space jumps.
/// The player turns to face the direction it is moving. Gravity and slopes/stairs are handled by the CharacterController.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7.5f;
    public float turnSpeed = 720f;       // degrees per second
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("References")]
    public Transform cameraTransform;    // movement is relative to this; defaults to the main camera

    // Used by RouteAutopilot so the walkthrough runs through exactly the same movement code as real play.
    [HideInInspector] public bool useExternalInput;
    [HideInInspector] public Vector3 externalWorldDirection;

    CharacterController controller;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector3 direction = useExternalInput ? Vector3.ClampMagnitude(externalWorldDirection, 1f) : ReadWorldDirection();
        bool sprint = !useExternalInput && Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float speed = sprint ? sprintSpeed : walkSpeed;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (!useExternalInput && controller.isGrounded && JumpPressed()) verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * speed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    // keyboard / gamepad input, turned into a world-space direction using the camera's heading
    Vector3 ReadWorldDirection()
    {
        Vector2 input = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
        }
        if (Gamepad.current != null) input += Gamepad.current.leftStick.ReadValue();
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 forward = cameraTransform != null ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized : transform.forward;
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        return forward * input.y + right * input.x;
    }

    static bool JumpPressed()
    {
        return (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
    }
}
