using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Rigidbody playerRigidbody;

    // Camera set variables
    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraMaxPitchAngle = 90f;
    private Vector2 lookVelocity;
    private float cameraXRotation;

    // Do Look Input
    public void InputLook(InputAction.CallbackContext context)
    {
        // Find and deliver Look Delta
        lookVelocity = context.ReadValue<Vector2>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // 
        DoPlayerLook();
    }

    void DoPlayerLook()
    {
        // Apply Player Rotation Yaw (Left / Right)
        Vector3 playerRotation = playerRigidbody.rotation.eulerAngles;
        float newYRotation = playerRotation.y + (lookVelocity.x * mouseSensitivity);
        playerRigidbody.rotation = Quaternion.Euler(playerRotation.x, newYRotation, playerRotation.z);

        // Send Warning if Missing Pivot Object
        if (playerCamera == null)
        {
            Debug.LogWarning("Missing Camera Pivot Object");
            return;
        }

        // Apply Player Rotation Pitch (Up / Down)
        cameraXRotation -= (lookVelocity.y * mouseSensitivity);
        cameraXRotation = Mathf.Clamp(cameraXRotation, -cameraMaxPitchAngle, cameraMaxPitchAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraXRotation, 0, 0);
    }
}
