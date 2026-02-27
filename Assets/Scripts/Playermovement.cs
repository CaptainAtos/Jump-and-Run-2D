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

    [Tooltip("Wenn true: beim Loslassen der Sprungtaste wird die Aufwärtsgeschwindigkeit gekürzt (kleiner Sprung).")]
    public bool cutJumpOnRelease = true;

    [Tooltip("Multiplikator für das 'Kürzen' beim Loslassen. 0.5 = halbiert die Aufwärtsgeschwindigkeit.")]
    [Range(0.1f, 1f)]
    public float jumpCutMultiplier = 0.5f;

    [Header("Coyote Time")]
    public bool useCoyoteTime = true;
    public float coyoteTime = 0.1f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer (Input speichern)")]
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
    public LayerMask wallLayer;
    private bool isGrounded;
    private bool isWalled;

    private void Update()
    {
<<<<<<< HEAD
=======
        // Boden prüfen
>>>>>>> 08c59d8 (Edited Playercontroller - Coyotetime und Jumpbuffer)
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer);

        isWalled = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            wallLayer);

<<<<<<< HEAD
=======
        // Jumps resetten, wenn am Boden
>>>>>>> 08c59d8 (Edited Playercontroller - Coyotetime und Jumpbuffer)
        if (isGrounded)
        {
            jumpsRemaining = Mathf.Max(1, maxJumps);
            if (useCoyoteTime) coyoteTimeCounter = coyoteTime;
        }
        else
        {
            if (useCoyoteTime) coyoteTimeCounter -= Time.deltaTime;
        }
        if (isWalled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

<<<<<<< HEAD
        }
        // Springen
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coyoteTimeCounter > 0f)
=======
        // Jump Buffer aktualisieren
        if (useJumpBuffer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                jumpBufferCounter = jumpBufferTime;
            else
                jumpBufferCounter -= Time.deltaTime;
        }

        // Jump auslösen
        bool wantsJump = Input.GetKeyDown(KeyCode.Space);
        if (useJumpBuffer)
            wantsJump = jumpBufferCounter > 0f;

        if (wantsJump)
        {
            if (TryJump())
>>>>>>> 08c59d8 (Edited Playercontroller - Coyotetime und Jumpbuffer)
            {
                // Buffer verbrauchen, damit nicht mehrfach ausgelöst wird
                if (useJumpBuffer) jumpBufferCounter = 0f;
                // Coyote-Time verbrauchen (damit nicht mehrfach)
                if (useCoyoteTime) coyoteTimeCounter = 0f;
            }
<<<<<<< HEAD
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // Double Jump verbraucht
=======
        }

        // Variable Sprunghöhe: Sprung kürzen beim Loslassen
        if (variableJumpHeight && cutJumpOnRelease && Input.GetKeyUp(KeyCode.Space))
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
>>>>>>> 08c59d8 (Edited Playercontroller - Coyotetime und Jumpbuffer)
            }
        }
    }

    private void FixedUpdate()
    {
        if (!rb) return;

        float input = Input.GetAxisRaw("Horizontal");

        // Sprint
        float currentMaxSpeed = maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMaxSpeed *= sprintMultiplier;
        }

<<<<<<< HEAD

        // Zielgeschwindigkeit berechnen
        float targetSpeed = input * currentMaxSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
=======
        // Zielgeschwindigkeit
        float targetSpeed = input * currentMaxSpeed;

        // Differenz zur aktuellen Geschwindigkeit
        float speedDifference = targetSpeed - rb.linearVelocity.x;

        // Beschleunigen/Bremsen
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

>>>>>>> 08c59d8 (Edited Playercontroller - Coyotetime und Jumpbuffer)
        float movement = speedDifference * accelRate * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement, rb.linearVelocity.y);

        // Schneller fallen
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
        // Ground jump wenn:
        // - grounded
        // - oder (CoyoteTime aktiv und Timer > 0)
        bool canGroundJump = isGrounded || (useCoyoteTime && coyoteTimeCounter > 0f);

        if (canGroundJump)
        {
            DoJump();
            // Bei Ground/Coyote Jump sollen wir danach noch (maxJumps-1) in der Luft haben.
            jumpsRemaining = Mathf.Max(1, maxJumps) - 1;
            return true;
        }

        // Air jump wenn noch Sprünge übrig
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
        // Y reset für konsistente Sprunghöhe
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

<<<<<<< HEAD
        // Neue Sprunggeschwindigkeit setzen
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);


=======
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );
>>>>>>> 08c59d8 (Edited Playercontroller - Coyotetime und Jumpbuffer)
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