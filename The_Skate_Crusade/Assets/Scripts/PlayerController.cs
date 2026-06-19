using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioSource pushSFX;
    private float pushSFXVol;
    [SerializeField] private AudioSource brakeSFX;
    private float brakeSFXVol;
    private Rigidbody playerRigidbody;
    private bool pushLeftPole;
    private bool pushRightPole;
    private bool brakeLeftPole;
    private bool brakeRightPole;
    private bool pressingAngleInput;
    private float playerVelocity;
    private float playerAngVelocity;
    private SpawnController _spawnController;

    // Animation Bits
    [Header("Animations")]
    [SerializeField] private Animator leftPoleAnimator;
    [SerializeField] private Animator rightPoleAnimator;
    [SerializeField] private Animator swordAnimator;
    public static bool inMenu;
    [SerializeField] private UIController UI;

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
    [SerializeField] private AudioSource SwordSFX;
    private float animStartTime;
    private float curSlashCD;
    private int slashNumber;
    private bool startSlashing;
    private bool slashing;
    public static float Health = 100.0f;
    public static int Score;

    [Header("Special Attack")]
    [SerializeField] private GameObject fireBall;
    private bool specialCharge;
    [SerializeField] private float specialTime;
    private float curSpecialTime;
    [SerializeField] private float specialCD;
    private float curSpecialCD;
    [SerializeField] private float chargeForwardSpped = 25f;
    public int coolPoints;
    [SerializeField] private int coolPointsPerCharge = 10;

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
        if (!startSlashing && !specialCharge)
        {
            startSlashing = context.performed;
            SwordSFX.pitch = Random.Range(0.8f, 1.3f);
            SwordSFX.Play();
        }
    }

    // Do Charge Attack Special
    public void ChargeAttack(InputAction.CallbackContext context)
    {
        // Attempt To Charge Attack
        if (!specialCharge && curSpecialCD <= 0 && coolPoints >= coolPointsPerCharge)
        {
            coolPoints -= coolPointsPerCharge;
            specialCharge = true;
            curSpecialTime = specialTime;
            curSpecialCD = specialCD;
        }
    }

    // Do Menu Toggle Button Input
    public void PlayerMenuInput(InputAction.CallbackContext context)
    {
        // Toggle Menu setting on 
        if (context.performed)
            inMenu = !inMenu;

        //
        UI.UpdateMenu();
        Time.timeScale = inMenu ? 0f : 1f;
        Cursor.visible = inMenu;
        Cursor.lockState = inMenu ? CursorLockMode.None : CursorLockMode.Confined;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //
        _spawnController = FindAnyObjectByType<SpawnController>();

        // Grab Rigidbody From GO
        playerRigidbody = GetComponent<Rigidbody>();

        // Lock Cursor By Default
        Cursor.lockState = CursorLockMode.Locked;

        //
        pushSFXVol = pushSFX.volume;
        brakeSFXVol = brakeSFX.volume;
        pushSFX.volume = 0f;
        brakeSFX.volume = 0f;

        // Reset Statics
        coolPoints = 0;
        Health = 100.0f;
        Score = 0;
        Time.timeScale = 1f;
        inMenu = false;
        UI.UpdateMenu();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Do Player Camera Rotation
        DoPlayerLook();

        // Do Pole Input
        DoPoleInput();

        // asd
        DoPoleSFX();

        //
        DoPlayerCharge();

        // Do Player Movement
        DoPlayerMovement();

        // Do Player Attack
        DoPlayerAttack();

        // Do Player Animations
        DoPoleAnimations();

        //
        if(Health <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
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
    void DoPoleSFX()
    {
        //
        float desPushVol = 0f;
        float desBrakeVol = 0f;

        //
        if (!slashing)
        {
            if (pushLeftPole || pushRightPole)
            {
                desPushVol = pushSFXVol;
            }
            if (brakeLeftPole || brakeRightPole)
            {
                desBrakeVol = brakeSFXVol;
            }
        }
        if (!pushLeftPole && !pushRightPole)
        {
            desPushVol = 0f;
        }
        if (!brakeLeftPole && !brakeRightPole)
        {
            desBrakeVol = 0f;
        }

        pushSFX.volume = Mathf.Lerp(pushSFX.volume, desPushVol, Time.fixedDeltaTime * 1.5f);
        brakeSFX.volume = Mathf.Lerp(brakeSFX.volume, desBrakeVol, Time.fixedDeltaTime * 1.5f);
    }

    //
    private void DoPlayerCharge()
    {
        fireBall.SetActive(specialCharge);

        //
        if (!specialCharge)
        {
            curSpecialCD -= Time.fixedDeltaTime;
            return;

        }

        //
        curSpecialTime -= Time.fixedDeltaTime;

        if(curSpecialTime <= 0)
        {
            specialCharge = false;
            return;
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
        playerVelocity = specialCharge ? chargeForwardSpped : playerVelocity;
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
        if((slashing || specialCharge) && other.tag == "Enemy")
        {
            Enemy otherScript = other.GetComponent<Enemy>();
            AddCoolPoint();
            otherScript.Death();
        }
    }

    public void AddCoolPoint()
    {
        if (!_spawnController.revoltStarted)
        {
            coolPoints++;
            coolPoints = Mathf.Clamp(coolPoints, 0, coolPointsPerCharge);
        }
    }
}
