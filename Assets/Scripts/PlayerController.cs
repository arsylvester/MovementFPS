//Player controller created by Andrew Sylvester. Enables the player to move, dash, slide, wall run, b hop, and strafe jump.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Base Movement Control")]
    [SerializeField] float accelerationAir = 5;
    [SerializeField] float accelerationGround = 5;
    [Tooltip("Cap on velocity with normal movement.")]
    [SerializeField] float maxVelocity = 10;
    [Tooltip("Ground friction")]
    [SerializeField] float friction = 1;
    [Tooltip("Ground friction increased")]
    [SerializeField] float frictionStopped = 2;
    [Tooltip("Slows down player if no input while in the air.")]
    [SerializeField] float airResistance = 1;
    public static float mouseSensitivity = .2f;
    [SerializeField] float lookVerticalMin = -85;
    [SerializeField] float lookVerticalMax = 85;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] bool canDoubleJump = false;
    [SerializeField] float doubleJumpHeight = .5f;
    [SerializeField] float CoyoteTime = .1f;
    [Header("Dash")]
    [SerializeField] float dashLength = 1.0f;
    [SerializeField] float dashCoolDown = 1.0f;
    [SerializeField] float DashVelocity = 5f;
    [SerializeField] GameObject dashParticles;
    [Header("Wall Running")]
    [SerializeField] float wallJumpForce = 1;
    [SerializeField] float wallRunSpeedCap = 1;
    [SerializeField] float wallRunSpeedMin = 1;
    [SerializeField] float wallRunSpeedMinBoost = 1;
    [Header("Sliding")]
    [SerializeField] float slideSpeed = 1;
    [SerializeField] float slideFastLength = 1f;
    [SerializeField] float crouchHeightPercent = .5f;
    [SerializeField] GameObject slideParticles;
    [Header("Head Tilt")]
    [Tooltip("Enables tilting of head on ground during movement. NOTE: Must have Tilt Head enabled as well.")]
    public bool tiltHeadGround = false;
    [Tooltip("Enables tilting head at all, wall or ground.")]
    public static bool tiltHead = true;
    [Tooltip("Angle to tilt at while on ground moving.")]
    [SerializeField] int groundTiltAngle = 5;
    [Tooltip("How much the camera should tilt while on a wall.")]
    [SerializeField] int wallRunTiltAngle = 10;
    [SerializeField] float tiltHeadSpeedGround = 1;
    [SerializeField] float tiltHeadSpeedWall = 1;
    [SerializeField] float headTiltAdditiveGround = .5f;
    [SerializeField] float headTiltAdditiveWall = .5f;
    [Header("Other")]
    [SerializeField] Weapon currentWeapon;
    [SerializeField] float weaponBobHeight;
    [SerializeField] float weaponBobSpeed;
    public static float fovValue = 90;
    [Header("Options")]
    public bool HoldJump = false;
    public bool HoldFire = true;
    Vector2 inputVector;
    Vector2 wishDirection;
    Vector3 movementVector;
    Vector2 cameraMovement;
    Vector2 currentVelocity;
    Vector2 prevSlideVelocity;
    Vector2 wallPositionLast;
    PlayerInput playerInput;
    CharacterController characterController;
    bool isGrounded;
    bool wasGrounded;
    bool isJump;
    bool hasJumped;
    bool hasDoublejumped;
    bool isSliding;
    bool startSliding;
    bool firstFrameGrounded = true;
    bool wallRight;
    bool OnWall;
    bool wallDetected;
    bool jumpedOffWall;
    bool isDashing;
    bool isFiring;
    bool canMove = true;
    bool crouchJump = false;
    bool jumpFirstPressed;
    float normalHeight;
    float currentDashTime;
    float currentSlideTime;
    float lastInput;
    float beforeDashVelocity;
    float timeFromGround;
    Vector3 WeaponBobOrignalPostion;
    Vector2 originalWallVelocity;
RaycastHit hitRight;
    RaycastHit hitLeft;
    Coroutine headtiltCoroutine;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        normalHeight = characterController.height;
        SetFOV(fovValue);
        WeaponBobOrignalPostion = currentWeapon.transform.localPosition;
    }
    private void Update()
    {
        if(isFiring)
        {
            currentWeapon.UseWeapon();
        }
    }

    void FixedUpdate()
    {
        //Check grounded for vertical movement
        isGrounded = characterController.isGrounded;
        if(isGrounded && movementVector.y < 0)
        {
            movementVector.y = 0;
            crouchJump = false;
            hasDoublejumped = false;
            hasJumped = false;
            jumpedOffWall = false;
        }

        if(wasGrounded != isGrounded && !isJump)
        {
            timeFromGround = Time.time;
        }
        wasGrounded = isGrounded;

        //Check to jump
        if (isJump && (isGrounded || timeFromGround + CoyoteTime >= Time.time))
        {
            if (!hasJumped)
            {
                movementVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                firstFrameGrounded = true;
                jumpFirstPressed = false;
                hasJumped = true;
                if (!HoldJump)
                {
                    isJump = false;
                }

                if (isSliding)
                {
                    crouchJump = true;
                }
            }
        }
        else if(canDoubleJump && isJump && !hasDoublejumped && jumpFirstPressed)
        {
            movementVector.y = Mathf.Sqrt(doubleJumpHeight * -3.0f * gravity);
            hasDoublejumped = true;
            print("Double jump");
        }

        //Not Dashing
        if (currentDashTime + dashLength < Time.time)
        {
            GetWishDirection();

            if (isDashing)
            {
                isDashing = false;
                //Allow dash into slide to maintain speed, espically on walls.
                if (!isSliding)
                {
                    movementVector = movementVector.normalized * beforeDashVelocity;
                }
                dashParticles.SetActive(false);
            }

            //Going using a Vector 2 for most calculations as y axis is not needed.
            currentVelocity.x = movementVector.x;
            currentVelocity.y = movementVector.z;

            if (isGrounded)
            {
                if(OnWall)
                {
                    //StartCoroutine(TiltHead(0, headTiltAdditiveWall, tiltHeadSpeedWall));
                    OnWall = false;
                }

                //Sliding on ground
                if (isSliding)
                {
                    //Provide speed boost in direction looking if standing still
                    if(startSliding)
                    {
                        startSliding = false;
                        GetDirectionLooking();
                        currentVelocity = wishDirection.normalized * slideSpeed;
                    }
                    //Give speed boost if speed is less than slideSpeed.
                    else if(currentVelocity.magnitude < slideSpeed || currentSlideTime + slideFastLength < Time.time)
                    {
                        currentVelocity = currentVelocity.normalized * slideSpeed;
                    }
                    //Might seperate slide length so that the slow down is more gradual then instant.
                }
                else
                {
                    //Only apply friction after the first frame, to allow speed to be reserved while bhopping
                    if (!firstFrameGrounded)
                    {
                        if (wishDirection.magnitude == 0)
                        {
                            currentVelocity = ApplyFriction(frictionStopped);
                        }
                        else
                        {
                            currentVelocity = ApplyFriction(friction);
                            currentVelocity = Accelerate(accelerationGround);
                        }
                    }

                    if (currentVelocity.magnitude >= .01f && !isSliding)
                    {
                        bobWeapon();
                    }
                    TiltHeadGround();
                }

                firstFrameGrounded = false;
                movementVector.y += gravity * Time.fixedDeltaTime;
            }
            else
            {
                //Wall running
                if ((characterController.collisionFlags & CollisionFlags.CollidedSides) != 0 || wallDetected)
                {

                    //Raycast to see if running on wall to right or left.

                    Transform wallHit = null;
                    if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitRight, 2))
                    {
                        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, 2))
                        {
                            if(hitLeft.distance < hitRight.distance)
                            {
                                wallRight = false;
                                wallHit = hitLeft.transform;
                            }
                            else
                            {
                                wallHit = hitRight.transform;
                                wallRight = true;
                            }
                        }
                        else
                        {
                            wallRight = true;
                            wallHit = hitRight.transform;
                        }
                        wallDetected = true;
                    }
                    else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, 2))
                    {
                        wallRight = false;
                        wallHit = hitLeft.transform;
                        wallDetected = true;
                    }
                    else
                    {
                        wallDetected = false;
                    }

                    bool canWallRun = true;

                    if(wallHit != null)
                    {
                        canWallRun = wallHit.tag != "Ignore Wall Run";
                    }
                    else
                    {
                        canWallRun = false;
                    }

                    if (canWallRun)
                    {
                        movementVector.y = 0;
                        //Sliding on wall
                        if (isSliding)
                        {
                            //Give speed boost if speed is less than slideSpeed.
                            if (currentVelocity.magnitude < slideSpeed || currentSlideTime + slideFastLength < Time.time)
                            {
                                currentVelocity = currentVelocity.normalized * slideSpeed;
                            }
                        }
                        else
                        {
                            //Get inital velocity
                            if(!OnWall)
                            {
                                originalWallVelocity = currentVelocity;
                            }

                            if(wallRight)
                            {
                                Vector2 rotatedWallNormal = new Vector2(hitRight.normal.z, -hitRight.normal.x);
                                if (currentVelocity.magnitude < wallRunSpeedMin)
                                {
                                    currentVelocity.x = rotatedWallNormal.normalized.x * wallRunSpeedMin;
                                    currentVelocity.y = rotatedWallNormal.normalized.y * wallRunSpeedMin;
                                }
                                else if (currentVelocity.magnitude > wallRunSpeedCap)
                                {
                                    currentVelocity.x = rotatedWallNormal.normalized.x * wallRunSpeedCap;
                                    currentVelocity.y = rotatedWallNormal.normalized.y * wallRunSpeedCap;
                                }
                                else
                                {
                                    float magnitude = currentVelocity.magnitude;
                                    currentVelocity.x = rotatedWallNormal.normalized.x * magnitude;
                                    currentVelocity.y = rotatedWallNormal.normalized.y * magnitude;
                                }
                            }
                            else
                            {
                                Vector2 rotatedWallNormal = new Vector3(-hitLeft.normal.z, hitLeft.normal.x);
                                if (currentVelocity.magnitude < wallRunSpeedMin)
                                {
                                    currentVelocity.x = rotatedWallNormal.normalized.x * wallRunSpeedMinBoost;
                                    currentVelocity.y = rotatedWallNormal.normalized.y * wallRunSpeedMinBoost;
                                }
                                else if(currentVelocity.magnitude > wallRunSpeedCap)
                                {
                                    currentVelocity.x = rotatedWallNormal.normalized.x * wallRunSpeedCap;
                                    currentVelocity.y = rotatedWallNormal.normalized.y * wallRunSpeedCap;
                                }
                                else
                                {
                                    float magnitude = currentVelocity.magnitude;
                                    currentVelocity.x = rotatedWallNormal.normalized.x * magnitude;
                                    currentVelocity.y = rotatedWallNormal.normalized.y * magnitude;
                                }
                            }
                            /*
                            if (characterController.velocity.magnitude < wallRunSpeedMin)
                            {
                                currentVelocity.x = currentVelocity.normalized.x * wallRunSpeedMinBoost ;
                                currentVelocity.y = currentVelocity.normalized.y * wallRunSpeedMinBoost;
                            }
                            else
                            {
                                currentVelocity.x = currentVelocity.normalized.x * wallRunSpeedCap ;
                                currentVelocity.y = currentVelocity.normalized.y * wallRunSpeedCap ;
                            }*/
                            wallPositionLast = new Vector2(transform.position.x, transform.position.z);
                        }

                        //To check if need to tilt to other side. Will remove if I decide not to be able to look the opposite way of running.
                        bool oldWallRight = wallRight;

                        //Actions for wall right or left
                        if (wallRight)
                        {
                            //On initial wall run
                            if (!OnWall || oldWallRight != wallRight)
                            {
                                TiltHead(wallRunTiltAngle, headTiltAdditiveWall, tiltHeadSpeedWall);
                            }
                        }
                        else
                        {
                            //On initial wall run
                            if (!OnWall || oldWallRight != wallRight)
                            {
                                TiltHead(-wallRunTiltAngle, headTiltAdditiveWall, tiltHeadSpeedWall);
                            }
                        }


                        if (currentVelocity.magnitude >= .01f && !isSliding && OnWall && !isJump)
                        {
                            bobWeapon();
                        }

                        //Jump off wall using the normal of the wall and an upwards force.
                        if (isJump)
                        {
                            //GetDirectionLooking();
                            if (wallRight)
                            {
                                /*float jumpPercentX = wishDirection.normalized.x / hitRight.normal.x;
                                float jumpPercentY = wishDirection.normalized.y / hitRight.normal.z;
                                float jumpPercentTotal = Mathf.Abs(jumpPercentX + jumpPercentY / 2);*/
                                currentVelocity.x += hitRight.normal.x * wallJumpForce;
                                currentVelocity.y += hitRight.normal.z * wallJumpForce;
                            }
                            else
                            {
                                currentVelocity.x += hitLeft.normal.x * wallJumpForce;
                                currentVelocity.y += hitLeft.normal.z * wallJumpForce;
                            }
                            wallDetected = false;
                            jumpFirstPressed = false;
                            timeFromGround = Time.time;
                            hasDoublejumped = false;
                            hasJumped = true;
                            TiltHead(0, headTiltAdditiveWall, tiltHeadSpeedWall);
                            movementVector.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                            jumpedOffWall = true;
                            print("Jumped off wall");
                        }

                        OnWall = true;
                    }
                    else
                    {
                        //If jumped off wall tilt head back and counter velocity that was going into wall.
                        //likely should change from wallJumpForce, but it seems to work for now.
                        if (OnWall)
                        {
                            TiltHead(0, headTiltAdditiveWall, tiltHeadSpeedWall);
                            /*
                            if (wallRight)
                            {
                                currentVelocity.x = hitRight.normal.x * wallJumpForce;
                                currentVelocity.y = hitRight.normal.z * wallJumpForce;
                            }
                            else
                            {
                                currentVelocity.x = hitLeft.normal.x * wallJumpForce;
                                currentVelocity.y = hitLeft.normal.z * wallJumpForce;
                            }*/
                        }
                        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                        OnWall = false;
                        currentVelocity = Accelerate(accelerationAir);
                        movementVector.y += gravity * Time.fixedDeltaTime;
                    }
                }
                //Not on ground nor a wall
                else
                {
                    //If jumped off wall tilt head back and counter velocity that was going into wall.
                    if (OnWall)
                    {
                       // StartCoroutine(TiltHead(0, headTiltAdditiveWall, tiltHeadSpeedWall));
                        /*if (wallRight)
                        {
                            currentVelocity.x = hitRight.normal.x * wallJumpForce;
                            currentVelocity.y = hitRight.normal.z * wallJumpForce;
                        }
                        else
                        {
                            currentVelocity.x = hitLeft.normal.x * wallJumpForce;
                            currentVelocity.y = hitLeft.normal.z * wallJumpForce;
                        }*/
                    }
                    //TiltHeadGround(); //Allows head tilt change with air direction, But causes issues with wall running head tilts for some reason.
                    OnWall = false;
                    if (wishDirection.magnitude == 0 && !crouchJump && !jumpedOffWall)
                    {
                        currentVelocity = ApplyFriction(airResistance);
                    }
                    else
                    {
                        currentVelocity = Accelerate(accelerationAir);
                    }
                    movementVector.y += gravity * Time.fixedDeltaTime;
                }
            }

            movementVector.x = currentVelocity.x;
            movementVector.z = currentVelocity.y;

        }
        else //Dash
        {
            movementVector.x = wishDirection.x * DashVelocity;
            movementVector.z = wishDirection.y * DashVelocity;
        }

        //Move the player dependent on code above.
        characterController.Move(movementVector);
        //print("Move Vector: " + movementVector.magnitude);
        //print("Char Control: " + characterController.velocity.magnitude);
    }

    private void GetWishDirection()
    {
        //Using theorem x2 = xcosB - ysinB
        //y2 = xsinB + ycosB
        //Unity works clockwise, math typically goes counterclockwise so use recipical? (360 - angle)
        float directionAngle = (360 - transform.eulerAngles.y) * Mathf.Deg2Rad;
        wishDirection.x = ((Mathf.Cos(directionAngle) * inputVector.x) - (Mathf.Sin(directionAngle) * inputVector.y));
        wishDirection.y = ((Mathf.Sin(directionAngle) * inputVector.x) + (Mathf.Cos(directionAngle) * inputVector.y));
    }


    //Acceleration caluclations
    //Mostly implemented from my own work but https://adrianb.io/2015/02/14/bunnyhop.html definitly helped, and of course Quake 3.
    private Vector2 Accelerate(float acceleration)
    {
        //float projectedVelocity = Vector2.Dot(currentVelocity, wishDirection.normalized);
        Vector2 acceleratedVelocity = ((acceleration * Time.fixedDeltaTime) * wishDirection.normalized) + currentVelocity;

        if (acceleratedVelocity.magnitude > maxVelocity)
        {
            acceleratedVelocity = acceleratedVelocity.normalized * maxVelocity;
        }
        return acceleratedVelocity;
    }

    //Slow player based on either ground friction or air resistance passed in.
    private Vector2 ApplyFriction(float currFriction)
    {
        Vector2 slowedVelocity = currentVelocity;
        float speed = currentVelocity.magnitude;
        if (speed != 0)
        {
            float slow = speed * currFriction * Time.fixedDeltaTime;
            slowedVelocity = currentVelocity * Mathf.Max(speed - slow, 0) / speed;
        }
        return slowedVelocity;
    }

    //Tilts heads based on current input.
    private void TiltHeadGround()
    {
        if (tiltHeadGround)
        {
            if (inputVector.x > 0 && lastInput <= 0)
            {
                TiltHead(-groundTiltAngle, headTiltAdditiveGround, tiltHeadSpeedGround);
                lastInput = inputVector.x;
            }
            else if (inputVector.x < 0 && lastInput >= 0)
            {
                TiltHead(groundTiltAngle, headTiltAdditiveGround, tiltHeadSpeedGround);
                lastInput = inputVector.x;
            }
            else if (inputVector.x == 0 && lastInput != 0)
            {
                TiltHead(0, headTiltAdditiveGround, tiltHeadSpeedGround);
                lastInput = inputVector.x;
            }
            
        }
    }

    private void TiltHead(float tiltAngle, float tiltAdd, float tiltSpeed)
    {
        if(headtiltCoroutine != null)
            StopCoroutine(headtiltCoroutine);
        headtiltCoroutine = StartCoroutine(TiltHeadCore(tiltAngle, tiltAdd, tiltSpeed));
    }

    private void bobWeapon()
    {
        float x = currentWeapon.transform.localPosition.x;
        float z = currentWeapon.transform.localPosition.z;
        currentWeapon.transform.localPosition = new Vector3(WeaponBobOrignalPostion.x + (Mathf.Sin(Time.time * weaponBobSpeed * .5f) * weaponBobHeight), WeaponBobOrignalPostion.y + (Mathf.Sin(Time.time * weaponBobSpeed) * weaponBobHeight), z);
    }

    //Move input action
   public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    //Mouse control input action
    public void OnLook(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            //Was using camera rotation, but that had some stutter issues. Rotating whole player seems to work. If issues down the line might have to revert back to adjusting camera.
            cameraMovement = context.ReadValue<Vector2>() * mouseSensitivity;
            Vector3 currentRotation = playerInput.camera.transform.localEulerAngles;

            //Add last frame angle with mousedelta and clamp the vertical. Mathf.clamp won't work with the angles so have to custom clamp it.
            float xRotation = (cameraMovement.y * mouseSensitivity * -1) + transform.localEulerAngles.x;
            if (transform.localEulerAngles.x < 90 && xRotation > lookVerticalMin)
            {
                xRotation = lookVerticalMin;
            }
            else if (transform.localEulerAngles.x > 270 && xRotation < lookVerticalMax)
            {
                xRotation = lookVerticalMax;
            }
            float yRotation = (cameraMovement.x * mouseSensitivity) + transform.localEulerAngles.y;

            //Rotate camera's x and player's y
            transform.localEulerAngles = new Vector3(xRotation, yRotation, transform.localEulerAngles.z);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            isJump = context.performed;
            jumpFirstPressed = true;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.ReadValueAsButton())
            {
                if (!isSliding)
                {
                    characterController.height = normalHeight * crouchHeightPercent;
                    isSliding = true;
                    startSliding = true;
                    currentSlideTime = Time.time;
                    slideParticles.SetActive(true);
                }
            }
            else
            {
                characterController.height = normalHeight;
                isSliding = false;
                slideParticles.SetActive(false);
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(canMove && context.ReadValueAsButton() && currentDashTime + dashLength + dashCoolDown < Time.time && !OnWall && !isSliding)
        {
            currentDashTime = Time.time;
            //If no direction dash forward.
            if(inputVector.magnitude == 0)
            {
                GetDirectionLooking();
            }
            else
            {
                GetWishDirection();
            }
            movementVector.y = 0;
            beforeDashVelocity = movementVector.magnitude;
            isDashing = true;
            dashParticles.SetActive(true);
        }
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.performed)
            {
                if (HoldFire)
                {
                    isFiring = true;
                }
                else
                {
                    currentWeapon.UseWeapon();
                }
            }
            else
            {
                isFiring = false;
            }
        }
    }

    bool holdAlt = false;
    public void OnAltFire(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.performed && !holdAlt)
            {
                currentWeapon.UseAltFireWeapon();
                holdAlt = true;
            }
            else if(holdAlt == true)
            {
                currentWeapon.AltFireWeaponRelease();
                holdAlt = false;
            }
        }
    }

    //Used to tilt heads for when on a wall or moving on ground if enabled. Currently a little too course as it moves the camera in ints over fixed time. If using ground camera tilt then need to smooth, but on the wall its okay.
    public IEnumerator TiltHeadCore(float angleToTilt, float tiltToAdd, float tiltSpeed)
    {
        float currentTilt = transform.localEulerAngles.z;
        if(currentTilt > 300)
        {
            currentTilt -= 360;
        }
        bool directionTilting = false;
        //Tilt right
        if(currentTilt < angleToTilt)
        {
            directionTilting = true;
        }

        while(currentTilt != angleToTilt && tiltHead)
        {
            //tilt right
            if (directionTilting)
            {
                currentTilt += tiltToAdd;
            }
            //tilt left
            else
            {
                currentTilt -= tiltToAdd;
            }

            if ((directionTilting && currentTilt >= angleToTilt) || (!directionTilting && currentTilt <= angleToTilt))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angleToTilt);
                break;
            }
            else
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTilt);
            }
            yield return new WaitForSeconds(tiltSpeed);
        }
    }

    //A way to get the wish direction with just the camera rotation.
    private void GetDirectionLooking()
    {
        float oldYInput = inputVector.y;
        inputVector.y = 1;
        GetWishDirection();
        inputVector.y = oldYInput;
    }

    //Used for UI
    public float GetCurrentSpeed()
    {
        return currentVelocity.magnitude;
    }

    //Used for UI of dash cooldown
    public float GetDashPercent()
    {
        return  (Time.time - currentDashTime) / (dashLength + dashCoolDown);
    }

    public void StopMovement()
    {
        movementVector = Vector3.zero;
        characterController.enabled = false;
        canMove = false;
    }
    public void ResumeMovement()
    {
        characterController.enabled = true;
        canMove = true;
    }

    public static void SetFOV(float value)
    {
        fovValue = value;
        Camera.main.fieldOfView = value;
    }
}
