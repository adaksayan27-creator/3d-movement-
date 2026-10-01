using UnityEngine;
using UnityEngine.InputSystem;

public class AimStateManager : MonoBehaviour
{
    [Header("Sensitivity & Limits")]
    [Tooltip("Adjust mouse look sensitivity")]
    public float mouseSensitivity = 0.15f;
    public float yMinAngle = -25f;
    public float yMaxAngle = 50f;
    public bool invertY = false;

    [Header("Camera Settings")]
    [Tooltip("Over-the-shoulder offset relative to player")]
    public Vector3 shoulderOffset = new Vector3(0.45f, 1.45f, 0f);
    [Tooltip("Distance behind the player")]
    public float cameraDistance = 3.2f;
    [Tooltip("Position smoothing time")]
    public float cameraSmoothTime = 0.03f;

    [Header("Current Angles (Read-Only)")]
    public float yaw;
    public float pitch;

    private Camera cam;
    private Vector3 cameraVelocity;

    void Awake()
    {
        cam = Camera.main;
        if (cam != null)
        {
            var brain = cam.GetComponent("CinemachineBrain") as Behaviour;
            if (brain != null)
            {
                brain.enabled = false;
            }
        }

        var cmCam = GameObject.Find("CinemachineCamera");
        if (cmCam != null)
        {
            cmCam.SetActive(false);
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = 12f;

        if (cam == null) cam = Camera.main;
        if (cam != null)
        {
            Quaternion camRot = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = transform.position + (Quaternion.Euler(0f, yaw, 0f) * shoulderOffset);
            cam.transform.position = pivot - (camRot * Vector3.forward * cameraDistance);
            cam.transform.rotation = camRot;
        }
    }

    void Update()
    {
        HandleCursorLock();
        HandleMouseLook();
    }

    void HandleCursorLock()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleMouseLook()
    {
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            yaw += mouseDelta.x * mouseSensitivity;

            float yChange = mouseDelta.y * mouseSensitivity * (invertY ? 1f : -1f);
            pitch += yChange;
            pitch = Mathf.Clamp(pitch, yMinAngle, yMaxAngle);
        }
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Quaternion camRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = transform.position + (Quaternion.Euler(0f, yaw, 0f) * shoulderOffset);
        Vector3 targetPos = pivot - (camRotation * Vector3.forward * cameraDistance);

        int groundMask = LayerMask.GetMask("Ground");
        if (groundMask == 0) groundMask = 1 << 6;
        if (Physics.SphereCast(pivot, 0.2f, (targetPos - pivot).normalized, out RaycastHit hit, cameraDistance, groundMask))
        {
            targetPos = pivot + (targetPos - pivot).normalized * Mathf.Max(hit.distance - 0.1f, 0.5f);
        }

        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPos, ref cameraVelocity, cameraSmoothTime);
        cam.transform.rotation = camRotation;
    }
}