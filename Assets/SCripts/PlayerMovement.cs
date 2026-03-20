using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    public Transform purlyVisual; // drag your visual child here

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movement input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        movement = new Vector2(h, v).normalized;

        // Visual rotation only (turn left/right around Y so he "looks around" instead of spinning)
        float turnInput = 0f;
        if (Input.GetKey(KeyCode.Q)) turnInput -= 1f;
        if (Input.GetKey(KeyCode.E)) turnInput += 1f;

        if (Mathf.Abs(turnInput) > 0f)
        {
            float delta = turnInput * rotationSpeed * Time.deltaTime;
            if (purlyVisual != null)
            {
                purlyVisual.Rotate(0f, delta, 0f);
            }
            else
            {
                transform.Rotate(0f, delta, 0f);
            }
        }
    }

    void FixedUpdate()
    {
        // Physics movement
        rb.linearVelocity = movement * moveSpeed;
    }
}
