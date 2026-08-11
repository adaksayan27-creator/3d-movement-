using UnityEngine;
using UnityEngine.InputSystem;

public class AimStateManager : MonoBehaviour
{
    public float xSensitivity = 0.3f;
    public float ySensitivity = 0.3f;
    public float yMinAngle = -80f;
    public float yMaxAngle = 80f;

    private float xValue;
    private float yValue;

    [SerializeField] Transform cameraFollowPos;

    void Start()
    {
        xValue = transform.localEulerAngles.y;
        yValue = -cameraFollowPos.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        xValue += mouseDelta.x * xSensitivity;
        yValue += mouseDelta.y * ySensitivity;
        yValue = Mathf.Clamp(yValue, yMinAngle, yMaxAngle);
    }

    private void LateUpdate()
    {
        cameraFollowPos.localEulerAngles = new Vector3(-yValue, cameraFollowPos.localEulerAngles.y, cameraFollowPos.localEulerAngles.z);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, xValue, transform.localEulerAngles.z);
    }
}