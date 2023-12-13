using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float DefaultSpeed, SprintSpeed, CurrentSpeed, SpeedLoss, SprintGain;
    [SerializeField] private float jumphight;
    private Rigidbody2D body;
    private bool grounded;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        CurrentSpeed += DefaultSpeed;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && grounded && CurrentSpeed < SprintSpeed)
        {
            CurrentSpeed += SprintGain;
        }
        else if (CurrentSpeed > DefaultSpeed)
        {
            CurrentSpeed -= SpeedLoss;
        }
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * CurrentSpeed, body.velocity.y); //Actual move part
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0) // Flippy flippy
        {
            transform.localRotation = Quaternion.Euler(0, horizontalInput > 0 ? 0 : 180, 0);
        }
        if (Input.GetKey(KeyCode.W) && grounded) Jump();
    }
    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumphight);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            grounded = true;
    }
}