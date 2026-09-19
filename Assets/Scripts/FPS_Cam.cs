using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPS_Cam : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement Speeds")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float crouchSpeed = 2f;

    [Header("Crouch Settings")]
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 8f;

    [Header("Jump / Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float lookClampAngle = 80f;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public float bobFrequency = 8f;
    public float bobAmount = 0.05f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private float currentHeight;
    private float targetHeight;
    private Vector3 originalCameraLocalPos;
    private float bobTimer;

    private bool isCrouching;
    private bool isRunning;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHeight = standHeight;
        targetHeight = standHeight;
        controller.height = currentHeight;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
            originalCameraLocalPos = cameraTransform.localPosition;
    }

    void Update()
    {
        if (Time.timeScale == 0f || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        HandleMouseLook();
        HandleCrouchInput();
        HandleMovement();
        HandleHeadBob();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookClampAngle, lookClampAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleCrouchInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            targetHeight = isCrouching ? crouchHeight : standHeight;
        }

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.height = currentHeight;

        Vector3 center = controller.center;
        center.y = currentHeight / 2f;
        controller.center = center;
    }

    void HandleMovement()
    {
        isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        float speed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = Vector3.ClampMagnitude(move, 1f);

        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump") && !isCrouching)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleHeadBob()
    {
        if (!enableHeadBob || cameraTransform == null) return;

        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (isMoving && controller.isGrounded)
        {
            float freq = isRunning ? bobFrequency * 1.5f : bobFrequency;
            bobTimer += Time.deltaTime * freq;

            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmount;
            cameraTransform.localPosition = originalCameraLocalPos + new Vector3(0f, bobOffsetY, 0f);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                originalCameraLocalPos,
                Time.deltaTime * 8f
            );
        }
    }
}