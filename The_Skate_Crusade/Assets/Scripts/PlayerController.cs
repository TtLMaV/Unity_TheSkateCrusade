using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //
    [Header("Movement")]
    [SerializeField] private float pushForwardSpeed = 5.0f;
    [SerializeField] private float pushRotationSpeed = 0.2f;
    [SerializeField] private float brakeBackwardsSpeed = 5.0f;
    [SerializeField] private float brakeRotationSpeed = 0.2f;
    [SerializeField] private float maxForwardSpeed = 15f;
    [SerializeField] private float maxAngleVelo = 1.0f;
    [SerializeField] private float AngularLoss = 0.1f;
    private Rigidbody playerRigidbody;
    private bool pushLeftPole;
    private bool pushRightPole;
    private bool brakeLeftPole;
    private bool brakeRightPole;
    private bool pressingMoveInput;
    private float playerVelocity;
    private float playerAngVelocity;
    
    // Camera set variables
    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraMaxHorizontalAngle = 30f;
    [SerializeField] private float cameraMaxPitchAngle = 90f;
    private Vector2 lookVelocity;
    private float cameraXRotation;

    // Do Look Input
    public void InputLook(InputAction.CallbackContext context)
    {
        // Find and deliver Look Delta
        lookVelocity = context.ReadValue<Vector2>();
    }

    // Do Left Skate Pole Push
    public void PushLeftSkate(InputAction.CallbackContext context)
    {
        // Attempt To Push Pole Via BOOL
        pushLeftPole = context.performed;
    }

    // Do Left Skate Pole Brake
    public void BrakeLeftSkate(InputAction.CallbackContext context)
    {
        // Attempt To Push Pole Via BOOL
        brakeLeftPole = context.performed;
    }

    // Do Left Skate Pole Push
    public void PushRightSkate(InputAction.CallbackContext context)
    {
        // Attempt To Push Pole Via BOOL
        pushRightPole = context.performed;
    }

    // Do Left Skate Pole Push
    public void BrakeRightSkate(InputAction.CallbackContext context)
    {
        // Attempt To Push Pole Via BOOL
        brakeRightPole = context.performed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grab Rigidbody From GO
        playerRigidbody = GetComponent<Rigidbody>();

        // Lock Cursor By Default
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Do Player Camera Rotation
        DoPlayerLook();

        // Do Pole Input
        DoPoleInput();

        // Do Player Movement
        DoPlayerMovement();
    }

    //
    void DoPlayerLook()
    {
        /*
        // Apply Player Rotation Yaw (Left / Right)
        Vector3 playerRotation = playerRigidbody.rotation.eulerAngles;
        float newYRotation = playerRotation.y + (lookVelocity.x * mouseSensitivity);
        playerRigidbody.rotation = Quaternion.Euler(playerRotation.x, newYRotation, playerRotation.z);
        */

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

    // 
    void DoPoleInput()
    {
        pressingMoveInput = pushLeftPole || pushRightPole || brakeLeftPole || brakeRightPole;

        // Left Pole Push
        if (pushLeftPole)
        {
            playerAngVelocity += pushRotationSpeed * Time.deltaTime;
            playerVelocity += pushForwardSpeed * Time.deltaTime;
        }

        // Right Pole Push
        if (pushRightPole)
        {
            playerAngVelocity -= pushRotationSpeed * Time.deltaTime;
            playerVelocity += pushForwardSpeed * Time.deltaTime;
        }

        // Left Pole Brake
        if (brakeLeftPole)
        {
            playerAngVelocity -= brakeRotationSpeed * Time.deltaTime;
            playerVelocity -= brakeBackwardsSpeed * Time.deltaTime;
        }

        // Right Pole Brake
        if (brakeRightPole)
        {
            playerAngVelocity += brakeRotationSpeed * Time.deltaTime;
            playerVelocity -= brakeBackwardsSpeed * Time.deltaTime;
        }
    }

    void DoPlayerMovement()
    {
        // Handle Player Movement
        playerVelocity = Mathf.Clamp(playerVelocity, 0, maxForwardSpeed);
        Vector3 forwardsSpeed = playerRigidbody.transform.forward * playerVelocity;
        playerRigidbody.linearVelocity = new Vector3(forwardsSpeed.x, playerRigidbody.linearVelocity.y, forwardsSpeed.z);
        playerRigidbody.angularVelocity = new Vector3(0f, playerAngVelocity, 0f);

        // Handle Min Max Ang Velo And Easing
        playerAngVelocity = Mathf.Clamp(playerAngVelocity, -maxAngleVelo, maxAngleVelo);
        if (!pressingMoveInput)
        {
            if (Mathf.Abs(playerAngVelocity) > AngularLoss * Time.deltaTime * 2f)
            {
                playerAngVelocity -= Mathf.Sign(playerAngVelocity) * Time.deltaTime * AngularLoss;
            }
            else
            {
                playerAngVelocity = 0;
            }
        }
    }
}
