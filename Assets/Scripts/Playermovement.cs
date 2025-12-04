using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Bewegung")]
    public float moveForce = 12f;         // Kraft für horizontale Bewegung
    public float maxSpeed = 5f;           // Normale Maximalgeschwindigkeit
    public float sprintMultiplier = 2f;   // Wie viel schneller beim Sprinten

    [Header("Springen")]
    public float jumpForce = 7f;          // Sprungkraft
    private bool canDoubleJump;           // Doppelsprung erlaubt?

    [Header("Fallen")]
    public float fallMultiplier = 2f;     // Schneller fallen

    [Header("Bodencheck")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Fast Stop")]
    [Range(0.05f, 0.5f)]
    public float brakeFactor = 0.2f;      // Geschwindigkeit beim Loslassen reduzieren

    void Update()
    {
        // -----------------------------
        // Bodencheck
        // -----------------------------
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = true; // Doppelsprung wieder verfügbar
        }

        // -----------------------------
        // Springen
        // -----------------------------
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // Doppelsprung verbraucht
            }
        }
    }

    void FixedUpdate()
    {
        // -----------------------------
        // Horizontale Bewegung
        // -----------------------------
        float input = Input.GetAxisRaw("Horizontal"); // A/D oder Pfeile

        // Sprint prüfen
        float currentMaxSpeed = maxSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMaxSpeed *= sprintMultiplier;
        }

        if (input != 0)
        {
            // Beschleunigen nur wenn unter MaxSpeed
            if (Mathf.Abs(rb.linearVelocity.x) < currentMaxSpeed)
            {
                rb.AddForce(new Vector2(input * moveForce, 0f), ForceMode2D.Force);
            }
        }
        else
        {
            // Fast Stop: Geschwindigkeit stark reduzieren
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * brakeFactor, rb.linearVelocity.y);
        }

        // -----------------------------
        // Schneller fallen
        // -----------------------------
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector2.down * fallMultiplier, ForceMode2D.Force);
        }
    }

    void Jump()
    {
        // Y-Geschwindigkeit zurücksetzen
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // Sprungkraft als Impuls
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // -----------------------------
    // Bodencheck-Gizmo
    // -----------------------------
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
