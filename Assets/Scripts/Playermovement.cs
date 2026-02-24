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
    private bool canDoubleJump;

    [Header("Coyote Time")]
    public float coyoteTime = 0.1f;   // Wie lange man nach Verlassen des Bodens noch springen darf
    private float coyoteTimeCounter;  // Timer der runterzählt

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

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer);

        isWalled = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            wallLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Timer zurücksetzen
            canDoubleJump = true;           // Double Jump wieder erlauben
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Timer runterzählen
        }
        if (isWalled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        }
        // Springen
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coyoteTimeCounter > 0f)
            {
                Jump();
                coyoteTimeCounter = 0f; // Verhindert mehrfaches Springen
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // Double Jump verbraucht
            }
        }
    }
    void FixedUpdate()
    {
        float input = Input.GetAxisRaw("Horizontal");

        // Sprint prüfen
        float currentMaxSpeed = maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMaxSpeed *= sprintMultiplier;
        }


        // Zielgeschwindigkeit berechnen
        float targetSpeed = input * currentMaxSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDifference * accelRate * Time.fixedDeltaTime;

        // Neue Geschwindigkeit setzen
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x + movement,
            rb.linearVelocity.y
        );

        // Schneller fallen
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y - fallMultiplier * Time.fixedDeltaTime
            );
        }
    }

    void Jump()
    {
        // Y-Geschwindigkeit zurücksetzen
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // Neue Sprunggeschwindigkeit setzen
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);


    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}