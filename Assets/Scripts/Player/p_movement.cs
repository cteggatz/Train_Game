using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class p_movement : MonoBehaviour
{
    //private InputAction moveAction;
    //public PlayerInput playerInput;
    PlayerControls controls;
    private Vector3 mousePos;
    private Rigidbody2D body;
    private Vector2 _moveInput;
    [SerializeField] private Animator animator;
    [SerializeField] private float lerpAmount, accelAmount, deccelAmount, accelInAir, deccelInAir, maxSpeed, jumphight, coyoteTimeInterval, jumpBufferInterval, sprintSpeed, normalSpeed;
    [Header("Gravity")]
    [SerializeField] private float gravityScale;
    [SerializeField] private float maxFallSpeed, fallGravityMult, f_FallGravityMult, maxFastFallSpeed, jumpHangTimeThreshold, jumpHangGravityMult, jumpHangAccelerationMult, jumpHangMaxSpeedMult, jumpCutGravityMult;
    private float timeLastOnGround, timeLastPressedJump;
    private bool grounded, IsJumping, _isJumpCut, _isJumpFalling, jumpKeyDown, sprintKeyDown;
    [SerializeField] private Animator squashAnimator;
    [SerializeField] private ParticleSystem jumpParticle;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        SetGravityScale(gravityScale);
        //playerInput = GetComponent<PlayerInput>();
        //moveAction = new InputAction("Horizontal");
    }

    private void OnEnable(){
        //moveAction.Enable();
        controls = new PlayerControls();
        //controls.PlayerMovementKeyboard.SetCallbacks();
        controls.PlayerMovementKeyboard.Horizontal.Enable();
    }
    private void OnDisable(){
        //moveAction.Disable();
        controls.PlayerMovementKeyboard.Disable();
    }

    private void FixedUpdate()
    {
        Run(1);
    }

    // Update is called once per frame
    void Update()
    {   //INPUT SETTINGS WHOOOOOOOOO
        //_moveInput.x = Input.GetAxisRaw("Horizontal");
        //_moveInput.y = Input.GetAxisRaw("Vertical");
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpKeyDown = true;
        }
        else { jumpKeyDown= false; }
        if (Input.GetKeyDown(KeyCode.LeftShift)) //Broken?!?!??!!
        {
            sprintKeyDown = true;
        }
        else { sprintKeyDown = false; }

        timeLastOnGround += Time.deltaTime;
        timeLastPressedJump -= Time.deltaTime;
        Vector2 scale = gameObject.transform.localScale;
        //gameObject.transform.localScale = new Vector2 (scale.x , scale.y + body.velocity.y/100);
        if (Time.time - timeLastOnGround <= coyoteTimeInterval){
            grounded = true;
        }
        animator.SetFloat("Speed", Mathf.Abs(_moveInput.x));//animation stuff
        if (gameObject.transform.position.x > mousePos.x) //looks in the direction of the mouse
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else{
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if(jumpKeyDown == true)
        {
            timeLastPressedJump = jumpBufferInterval;
        }
        else if(jumpKeyDown == false) {
            _isJumpCut = true;
        }
        if (sprintKeyDown == true)
        {
            animator.SetBool("Sprinting", true);
            accelAmount = sprintSpeed;
            gameObject.GetComponent<PlayerInventory>().itemRenderer.gameObject.SetActive(false);
            gameObject.GetComponent<PlayerInventory>().enabled = false;

        }
        else if (sprintKeyDown == false)
        {
            animator.SetBool("Sprinting", false);
            accelAmount = normalSpeed;
            gameObject.GetComponent<PlayerInventory>().enabled = true;
            gameObject.GetComponent<PlayerInventory>().itemRenderer.gameObject.SetActive(true);
        }
        if (grounded && timeLastPressedJump > 0){
            timeLastPressedJump = 0;
            IsJumping = true;
            grounded = false;
            squashAnimator.SetTrigger("Jump");
            Instantiate(jumpParticle).transform.position = squashAnimator.gameObject.transform.position;
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
        //_moveInput = moveAction.ReadValue<Vector2>();
        //Debug.Log(moveAction.ReadValue<Vector2>());
        //Calculate the direction
        //_moveInput = playerInput.ReadValue<Vector2>();
        _moveInput = controls.PlayerMovementKeyboard.Horizontal.ReadValue<Vector2>();
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
            squashAnimator.SetTrigger("Landing");
            timeLastOnGround = 0;
            grounded = true;
        }
    public void SetGravityScale(float scale)
    {
        body.gravityScale = scale;
    }

}
