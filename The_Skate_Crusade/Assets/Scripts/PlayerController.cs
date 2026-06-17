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
    private bool pressingAngleInput;
    private float playerVelocity;
    private float playerAngVelocity;

    // Animation Bits
    [Header("Animations")]
    [SerializeField] private Animator leftPoleAnimator;
    [SerializeField] private Animator rightPoleAnimator;
    [SerializeField] private Animator swordAnimator;

    // Camera set variables
    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraMaxHorizontalAngle = 30f;
    [SerializeField] private float cameraMaxPitchAngle = 90f;
    private Vector2 lookVelocity;
    private float cameraXRotation;
    private float cameraYRotation;

    [Header("Sword")]
    [SerializeField] private float slashCD = 5.0f;
    [SerializeField] private int finalSlashNumber = 1;
    private float animStartTime;
    private float curSlashCD;
    private int slashNumber;
    private bool startSlashing;
    private bool slashing;

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

    // Do Left Skate Pole Push
    public void SwordAttack(InputAction.CallbackContext context)
    {
        // Attempt To Push Pole Via BOOL
        if (!startSlashing)
        {
            startSlashing = context.performed;
        }
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
    void FixedUpdate()
    {
        // Do Player Camera Rotation
        DoPlayerLook();

        // Do Pole Input
        DoPoleInput();

        // Do Player Movement
        DoPlayerMovement();

        // Do Player Attack
        DoPlayerAttack();

        // Do Player Animations
        DoPoleAnimations();
    }

    //
    void DoPlayerLook()
    {
        // Send Warning if Missing Pivot Object
        if (playerCamera == null)
        {
            Debug.LogWarning("Missing Camera Pivot Object");
            return;
        }

        //
        cameraYRotation += (lookVelocity.x * mouseSensitivity);
        cameraYRotation = Mathf.Clamp(cameraYRotation, -cameraMaxHorizontalAngle, cameraMaxHorizontalAngle);

        // Apply Player Rotation Pitch (Up / Down)
        cameraXRotation -= (lookVelocity.y * mouseSensitivity);
        cameraXRotation = Mathf.Clamp(cameraXRotation, -cameraMaxPitchAngle, cameraMaxPitchAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraXRotation, cameraYRotation, 0);
    }

    // 
    void DoPoleInput()
    {
        //
        if (slashing)
        {
            //
            pressingAngleInput = false;
        }
        else
        {
            // 
            pressingAngleInput = (pushLeftPole != pushRightPole) || (brakeLeftPole != brakeRightPole);

            // Left Pole Push
            if (pushLeftPole)
            {
                playerAngVelocity += pushRotationSpeed * Time.fixedDeltaTime;
                playerVelocity += pushForwardSpeed * Time.fixedDeltaTime;
            }

            // Right Pole Push
            if (pushRightPole)
            {
                playerAngVelocity -= pushRotationSpeed * Time.fixedDeltaTime;
                playerVelocity += pushForwardSpeed * Time.fixedDeltaTime;
            }

            // Left Pole Brake
            if (brakeLeftPole)
            {
                playerAngVelocity -= brakeRotationSpeed * Time.fixedDeltaTime;
                playerVelocity -= brakeBackwardsSpeed * Time.fixedDeltaTime;
            }

            // Right Pole Brake
            if (brakeRightPole)
            {
                playerAngVelocity += brakeRotationSpeed * Time.fixedDeltaTime;
                playerVelocity -= brakeBackwardsSpeed * Time.fixedDeltaTime;
            }
        }
    }

    //
    void DoPlayerMovement()
    {
        if(playerRigidbody.linearVelocity.magnitude - playerVelocity <= -1f)
        {
            playerVelocity = playerRigidbody.linearVelocity.magnitude;
        }

        // Handle Player Movement
        playerVelocity = Mathf.Clamp(playerVelocity, -2f, maxForwardSpeed);
        Vector3 forwardsSpeed = playerRigidbody.transform.forward * playerVelocity;
        playerRigidbody.linearVelocity = new Vector3(forwardsSpeed.x, playerRigidbody.linearVelocity.y, forwardsSpeed.z);
        playerRigidbody.angularVelocity = new Vector3(0f, playerAngVelocity, 0f);

        // Handle Min Max Ang Velo And Easing
        playerAngVelocity = Mathf.Clamp(playerAngVelocity, -maxAngleVelo, maxAngleVelo);
        if (!pressingAngleInput)
        {
            if (Mathf.Abs(playerAngVelocity) > AngularLoss * Time.fixedDeltaTime * 2f)
            {
                playerAngVelocity -= Mathf.Sign(playerAngVelocity) * Time.fixedDeltaTime * AngularLoss;
            }
            else
            {
                playerAngVelocity = 0;
            }
        }
    }

    //
    void DoPlayerAttack()
    {
        

        // Detect End Of Combo Chance
        if (curSlashCD <= 0)
        {
            slashNumber = 0;
        }

        // Detect Start Of Slash
        if (startSlashing && !slashing)
        {
            //
            animStartTime = Time.time;
            slashing = true;
            slashNumber++;
            slashNumber = Mathf.Clamp(slashNumber, 0, finalSlashNumber);

            //
            curSlashCD = slashCD;
        }

        //
        string currentAnim = swordAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        float animTime = (Time.time - animStartTime) / 1f;

        // Detect End Of Main Animation
        if (currentAnim != "Sword_Idle" && animTime > 0.65f && startSlashing)
        {
            startSlashing = false;
            slashing = false;

            // Reset If At Final Slash
            if (slashNumber == finalSlashNumber)
                slashNumber = 0;
        }

        // Countdown Timer Till Combo End
        if (curSlashCD > 0 && !startSlashing)
        {
            curSlashCD -= Time.fixedDeltaTime;
        }
    }

    //
    void DoPoleAnimations()
    {
        // Animate Left Pole
        leftPoleAnimator.SetBool("Braking", brakeLeftPole);
        leftPoleAnimator.SetBool("Pushing", pushLeftPole);
        leftPoleAnimator.SetBool("Attacking", slashing);

        // Animate Right Pole
        rightPoleAnimator.SetBool("Braking", brakeRightPole);
        rightPoleAnimator.SetBool("Pushing", pushRightPole);
        rightPoleAnimator.SetBool("Attacking", slashing);

        // Animate Sword
        swordAnimator.SetInteger("Swing", slashNumber);
        float animTime = (Time.time - animStartTime) / 1f;
        animTime = Mathf.Clamp(animTime, 0f, 0.98f);
        swordAnimator.SetFloat("TTime", animTime);

    }

    // 
    private void OnTriggerEnter(Collider other)
    {
        if(slashing && other.tag == "Enemy")
        {
            Enemy otherScript = other.GetComponent<Enemy>();
            otherScript.Death();
        }
    }
}
