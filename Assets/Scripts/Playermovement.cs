using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [SerializeField] private float Speed;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(horizontalInput, body.linearVelocity.y);


        if (Input.GetKey(KeyCode.Space))
            body.linearVelocity = new Vector2(body.linearVelocity.x, Speed);

    }



} 
