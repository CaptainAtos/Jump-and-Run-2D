using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Bewegung")]
    public float maxSpeed = 3f;
    public float sprintMultiplier = 2f;
    public float acceleration = 15f;
    public float deceleration = 20f;

    [Header("Springen")]
    public float jumpForce = 12f;

    [Tooltip("1 = kein Mehrfachsprung, 2 = Double Jump, 3 = Triple Jump, ...")]
    public int maxJumps = 2;

    private int jumpsRemaining;

    [Header("Variable Sprunghöhe (Taste halten)")]
    public bool variableJumpHeight = true;

    [Tooltip("Wenn true: beim Loslassen der Sprungtaste wird die Aufwärtsgeschwindigkeit gekürzt.")]
    public bool cutJumpOnRelease = true;

    [Range(0.1f, 1f)]
    public float jumpCutMultiplier = 0.5f;

    [Header("Coyote Time")]
    public bool useCoyoteTime = true;
    public float coyoteTime = 0.1f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    public bool useJumpBuffer = true;
    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [Header("Fallen")]
    public float fallMultiplier = 25f;

    [Header("Bodencheck")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;

    private void Start()
    {
        jumpsRemaining = Mathf.Max(1, maxJumps);
    }

    private void Update()
    {
        // Boden prüfen
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        // Jumps resetten
        if (isGrounded)
        {
            jumpsRemaining = Mathf.Max(1, maxJumps);
            if (useCoyoteTime) coyoteTimeCounter = coyoteTime;
        }
        else
        {
            if (useCoyoteTime)
                coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (useJumpBuffer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                jumpBufferCounter = jumpBufferTime;
            else
                jumpBufferCounter -= Time.deltaTime;
        }

        bool wantsJump = Input.GetKeyDown(KeyCode.Space);
        if (useJumpBuffer)
            wantsJump = jumpBufferCounter > 0f;

        if (wantsJump)
        {
            if (TryJump())
            {
                if (useJumpBuffer) jumpBufferCounter = 0f;
                if (useCoyoteTime) coyoteTimeCounter = 0f;
            }
        }

        // Variable Jump Height
        if (variableJumpHeight && cutJumpOnRelease && Input.GetKeyUp(KeyCode.Space))
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
            }
        }
    }

    private void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");

        float currentMaxSpeed = maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            currentMaxSpeed *= sprintMultiplier;

        float targetSpeed = input * currentMaxSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDifference * accelRate * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x + movement,
            rb.linearVelocity.y
        );

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y - fallMultiplier * Time.fixedDeltaTime
            );
        }
    }

    private bool TryJump()
    {
        bool canGroundJump = isGrounded || (useCoyoteTime && coyoteTimeCounter > 0f);

        if (canGroundJump)
        {
            DoJump();
            jumpsRemaining = Mathf.Max(1, maxJumps) - 1;
            return true;
        }

        if (jumpsRemaining > 0)
        {
            DoJump();
            jumpsRemaining--;
            return true;
        }

        return false;
    }

    private void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}