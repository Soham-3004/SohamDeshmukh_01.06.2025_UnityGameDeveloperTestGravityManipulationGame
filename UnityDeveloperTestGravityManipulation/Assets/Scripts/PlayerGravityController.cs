using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravityController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Rigidbody rb;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public LayerMask groundLayer;

    [Header("Custom Gravity")]
    public float gravityStrength = 30f;
    private Vector3[] gravityDirections = new Vector3[]
    {
        Vector3.down, Vector3.up, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
    };
    private Vector3 customGravityDir = Vector3.down;
    private int currentGravityIndex = 0;
    private Vector3 storedGravityDirection;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.8f;

    [Header("Holo Visual")]
    public GameObject holoObject;
    public float offsetDistance = 2.5f;
    public float verticalOffset = 2.5f;
    public float mouseThreshold = 5f;

    private Vector3 lastMousePosition;
    private Transform playerTransform;

    [Header("Animations")]
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerTransform = transform;

        rb.useGravity = false; // Disable Unity's gravity

        if (holoObject == null)
            Debug.LogError("HoloObject not assigned!");

        SetGravityDirection(gravityDirections[currentGravityIndex]);
    }

    void Update()
    {
        HandleMovement();
        HandleGravityMouseInput();
        HandleJump();
        HandleFallingAnimation();
    }

    void FixedUpdate()
    {
        ApplyCustomGravity();
    }

    void ApplyCustomGravity()
    {
        rb.AddForce(customGravityDir * gravityStrength, ForceMode.Acceleration);
    }

    public bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics.Raycast(groundCheck.position, -groundCheck.forward, 1f, groundLayer);
    }
    private void UpdateGroundCheckTransform()
    {
        if (groundCheck == null)
            return;

        // Position it a fixed distance away from the player in the direction of gravity
        groundCheck.position = transform.position + (-customGravityDir.normalized * groundCheckDistance);

        // Rotate the groundCheck so its forward points opposite to gravity
        groundCheck.rotation = Quaternion.LookRotation(-customGravityDir, transform.up);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Vector3 jumpDir = -customGravityDir;
            rb.velocity = Vector3.zero;
            rb.AddForce(jumpDir * jumpForce, ForceMode.VelocityChange);
        }
    }

    void HandleFallingAnimation()
    {
        bool grounded = IsGrounded();
        animator.SetBool("isFalling", !grounded);
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Local right and forward based on current gravity
        Vector3 right = transform.right;
        Vector3 forward = Vector3.Cross(customGravityDir, right).normalized;

        Vector3 movement = (right * moveHorizontal + forward * moveVertical).normalized;

        Vector3 velocity = rb.velocity;
        Vector3 gravityComponent = Vector3.Project(velocity, customGravityDir);
        Vector3 newVelocity = movement * moveSpeed + gravityComponent;

        rb.velocity = newVelocity;

        animator.SetBool("isRunning", movement.magnitude > 0.1f && IsGrounded());
    }

    void HandleGravityMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
            storedGravityDirection = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            Vector3 desiredDirection = GetGravityDirectionFromMouse(mouseDelta);

            if (desiredDirection != Vector3.zero)
            {
                storedGravityDirection = desiredDirection;  // Just store it
                SpawnHolo(desiredDirection);
            }

            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (storedGravityDirection != Vector3.zero)
            {
                int index = System.Array.FindIndex(gravityDirections, dir => Vector3.Angle(dir, storedGravityDirection) < 5f);
                if (index != -1 && index != currentGravityIndex)
                {
                    currentGravityIndex = index;
                    SetGravityDirection(gravityDirections[currentGravityIndex]);
                }
            }

            if (holoObject.activeSelf)
                holoObject.SetActive(false);
        }
    }

    void SetGravityDirection(Vector3 newDirection)
    {
        customGravityDir = newDirection.normalized;

        // Keep only velocity along gravity axis
        Vector3 velocity = rb.velocity;
        Vector3 gravityComponent = Vector3.Project(velocity, customGravityDir);
        rb.velocity = gravityComponent;

        // Align character's up to match new gravity
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -customGravityDir) * transform.rotation;
        transform.rotation = targetRotation;

        UpdateGroundCheckTransform();
        CustomGravityForCamera.SetGravity(customGravityDir);
    }

    Vector3 GetGravityDirectionFromMouse(Vector3 mouseDelta)
    {
        if (mouseDelta.x > mouseThreshold)
            return playerTransform.right;
        else if (mouseDelta.x < -mouseThreshold)
            return -playerTransform.right;
        else if (mouseDelta.y > mouseThreshold)
            return playerTransform.forward;
        else if (mouseDelta.y < -mouseThreshold)
            return -playerTransform.forward;

        return Vector3.zero;
    }

    void SpawnHolo(Vector3 direction)
    {
        if (!holoObject.activeSelf)
            holoObject.SetActive(true);

        Vector3 spawnPos = playerTransform.position + transform.up * verticalOffset + direction * offsetDistance;
        holoObject.transform.position = spawnPos;

        Vector3 lookDirection = (playerTransform.position + transform.up * verticalOffset) - spawnPos;
        holoObject.transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        holoObject.transform.Rotate(90f, 0f, 0f); // Adjust for mesh
    }

}
