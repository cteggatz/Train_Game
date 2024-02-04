using UnityEngine;

public class p_movement : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 velocity, _moveInput;
    [SerializeField] private Animator animator;
    [SerializeField] private float lerpAmount, accelAmount, deccelAmount, accelInAir, deccelInAir, maxSpeed, jumphight, coyoteTimeInterval, jumpBufferInterval;
    [Header("Gravity")]
    [SerializeField] private float gravityScale;
    [SerializeField] private float maxFallSpeed, fallGravityMult, f_FallGravityMult, maxFastFallSpeed, jumpHangTimeThreshold, jumpHangGravityMult, jumpHangAccelerationMult, jumpHangMaxSpeedMult, jumpCutGravityMult;
    private float timeLastOnGround, timeLastPressedJump;
    private bool grounded, IsJumping, _isJumpCut, _isJumpFalling;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        SetGravityScale(gravityScale);
    }

    // Update is called once per frame
    void Update()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        timeLastOnGround += Time.deltaTime;
        timeLastPressedJump -= Time.deltaTime;
        Run(1);
        if (Time.time - timeLastOnGround <= coyoteTimeInterval){
            grounded = true;
        }
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));//animation stuff
        if (gameObject.transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x) //looks in the direction of the mouse
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else{
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            timeLastPressedJump = jumpBufferInterval;
        }
        if(Input.GetKeyUp(KeyCode.W)) {
            _isJumpCut = true;
        }
        if (grounded && timeLastPressedJump > 0){
            timeLastPressedJump = 0;
            IsJumping = true;
            grounded = false;
            body.velocity = new Vector2(body.velocity.x, jumphight);
        }
        if (IsJumping && body.velocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }


        if (timeLastOnGround > 0 && !IsJumping)
        {
            _isJumpCut = false;
            if (!IsJumping)
                _isJumpFalling = false;
        }

        //gravity
        if (body.velocity.y < 0 && _moveInput.y < 0)
        {
            //Much higher gravity if holding down
            SetGravityScale(gravityScale * f_FallGravityMult);
            //Caps max fall speed
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            //Higher gravity if jump button is released
            SetGravityScale(gravityScale * jumpCutGravityMult);
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, maxFallSpeed));
        }
        else if ((IsJumping || _isJumpFalling) && Mathf.Abs(body.velocity.y) < jumpHangTimeThreshold)
        {
            SetGravityScale(gravityScale * jumpHangGravityMult);
        }
        else if (body.velocity.y < 0)
        {
            //Higher gravity when falling
            SetGravityScale(gravityScale * fallGravityMult);
            //Caps max fall speed
            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, maxFallSpeed));
        }
        else
        {
            //Default gravity when on ground or moving upwards
            SetGravityScale(gravityScale);
        }
    }
    private void Run(float lerpAmount)
    {
        //Calculate the direction
        float targetSpeed = _moveInput.x * maxSpeed;
        //Reduce player controll
        targetSpeed = Mathf.Lerp(body.velocity.x, targetSpeed, lerpAmount);
        float accelRate;

        //Gets an acceleration value
        if (timeLastOnGround > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelAmount : deccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelAmount * accelInAir : deccelAmount * deccelInAir;

        //Make the jump a bit more bouncy? idk people like this for some reason
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(body.velocity.y) < jumpHangTimeThreshold)
        {
            accelRate *= jumpHangAccelerationMult;
            targetSpeed *= jumpHangMaxSpeedMult;
        }

        //B-hopping allowed
        if (Mathf.Abs(body.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(body.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && timeLastOnGround < 0)
        {
            accelRate = 0;
        }
        //acctually add the final force
        body.AddForce((targetSpeed - body.velocity.x) * accelRate * Vector2.right, ForceMode2D.Force);
    }
        private void OnCollisionEnter2D(Collision2D collision)
    {
        timeLastOnGround = 0;
        grounded = true;
    }
    public void SetGravityScale(float scale)
    {
        body.gravityScale = scale;
    }

}
