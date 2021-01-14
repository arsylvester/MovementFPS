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
    [Tooltip("Slows down player if no input while in the air.")]
    [SerializeField] float airResistance = 1;
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float lookVerticalMin = -85;
    [SerializeField] float lookVerticalMax = 85;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.0f;
    [Header("Dash")]
    [SerializeField] float dashLength = 1.0f;
    [SerializeField] float dashCoolDown = 1.0f;
    [SerializeField] float DashVelocity = 5f;
    [SerializeField] GameObject dashParticles;
    [Header("Wall Running")]
    [SerializeField] float wallJumpForce = 1;
    [SerializeField] float wallRunSpeedCap = 1;
    [Tooltip("How much the camera should tilt while on a wall.")]
    [SerializeField] int wallRunTiltAngle = 10;
    [Header("Sliding")]
    [SerializeField] float slideSpeed = 1;
    [SerializeField] float slideFastLength = 1f;
    [SerializeField] float crouchHeightPercent = .5f;
    [SerializeField] GameObject slideParticles;
    [Header("Other")]
    [Tooltip("Angle to tilt at while on ground moving.")]
    [SerializeField] int groundTiltAngle = 5;
    [SerializeField] Weapon currentWeapon;
    [Header("Options")]
    [Tooltip("Enables tilting head at all, wall or ground.")]
    public bool tiltHead = true;
    [Tooltip("Enables tilting of head on ground during movement. NOTE: Must have Tilt Head enabled as well.")]
    public bool tiltHeadGround = false;
    public bool HoldJump = false;
    public bool HoldFire = true;
    Vector2 inputVector;
    Vector2 wishDirection;
    Vector3 movementVector;
    Vector2 cameraMovement;
    Vector2 currentVelocity;
    PlayerInput playerInput;
    CharacterController characterController;
    bool isGrounded;
    bool isJump;
    bool isSliding;
    bool firstFrameGrounded = true;
    bool wallRight;
    bool OnWall;
    float normalHeight;
    float currentDashTime;
    float currentSlideTime;
    float lastInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        normalHeight = characterController.height;
    }

    void FixedUpdate()
    {
        //Check grounded for vertical movement
        isGrounded = characterController.isGrounded;
        if(isGrounded && movementVector.y < 0)
        {
            movementVector.y = 0;
        }

        //Check to jump
        if(isJump && isGrounded)
        {
            movementVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            firstFrameGrounded = true;
            if(!HoldJump)
            {
                isJump = false;
            }
        }

        //Not Dashing
        if (currentDashTime + dashLength < Time.time)
        {
            GetWishDirection();

            //Going using a Vector 2 for most calculations as y axis is not needed.
            currentVelocity.x = movementVector.x;
            currentVelocity.y = movementVector.z;

            dashParticles.SetActive(false);

            if (isGrounded)
            {
                //Sliding on ground
                if (isSliding)
                {
                    //Provide speed boost in direction looking if standing still
                    if(currentVelocity.magnitude <= .01f)
                    {
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
                        currentVelocity = ApplyFriction(friction);
                    }
                    currentVelocity = Accelerate(accelerationGround);
                    TiltHeadGround();
                }

                firstFrameGrounded = false;
                movementVector.y += gravity * Time.fixedDeltaTime;
                OnWall = false;
            }
            else
            {
                //Wall running
                if ((characterController.collisionFlags & CollisionFlags.CollidedSides) != 0)
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
                        currentVelocity.x = currentVelocity.normalized.x * wallRunSpeedCap;
                        currentVelocity.y = currentVelocity.normalized.y * wallRunSpeedCap;
                    }

                    //To check if need to tilt to other side. Will remove if I decide not to be able to look the opposite way of running.
                    bool oldWallRight = wallRight;

                    //Raycast to see if running on wall to right or left.
                    RaycastHit hitRight;
                    RaycastHit hitLeft;
                    if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitRight, 10))
                    {
                        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, 10))
                        {
                            if(hitLeft.distance < hitRight.distance)
                            {
                                wallRight = false;
                            }
                            else
                            {
                                wallRight = true;
                            }
                        }
                        else
                        {
                            wallRight = true;
                        }
                    }
                    else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitLeft, 10))
                    {
                        wallRight = false;
                    }

                    //Actions for wall right or left
                    if(wallRight)
                    {
                        //On initial wall run
                        if (!OnWall || oldWallRight != wallRight)
                        {
                            StartCoroutine(TiltHead(wallRunTiltAngle));
                        }
                    }
                    else
                    {
                        //On initial wall run
                        if (!OnWall || oldWallRight != wallRight)
                        {
                            StartCoroutine(TiltHead(-wallRunTiltAngle));
                        }
                    }

                    //Jump off wall using the normal of the wall and an upwards force.
                    if(isJump)
                    {
                        if(wallRight)
                        {
                            currentVelocity.x = hitRight.normal.x * wallJumpForce;
                            currentVelocity.y = hitRight.normal.z * wallJumpForce;
                            print(hitRight.normal);
                        }
                        else
                        {
                            currentVelocity.x = hitLeft.normal.x * wallJumpForce;
                            currentVelocity.y = hitLeft.normal.z * wallJumpForce;
                        }
                        StartCoroutine(TiltHead(0));
                        movementVector.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                    }

                    OnWall = true;
                }
                //Not on ground nor a wall
                else
                {
                    //If jumped off wall tilt head back
                    if(OnWall)
                    {
                        StartCoroutine(TiltHead(0));
                    }
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
                    OnWall = false;
                    currentVelocity = Accelerate(accelerationAir);
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
        float projectedVelocity = Vector2.Dot(currentVelocity, wishDirection.normalized);
        float acceleratedVelocity = acceleration * Time.fixedDeltaTime;

        if (acceleratedVelocity + projectedVelocity > maxVelocity)
        {
            acceleratedVelocity = maxVelocity - projectedVelocity;
        }
        //Trying changing this to always be in the wishdirection, but now always caps at max velocity, and stops too abrubtly. 
        if (wishDirection.magnitude == 0)
        {
            return ApplyFriction(airResistance);
        }
        else
        {
            return currentVelocity + wishDirection.normalized * acceleratedVelocity;
        }
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
                StartCoroutine(TiltHead(-groundTiltAngle));
            }
            else if (inputVector.x < 0 && lastInput >= 0)
            {
                StartCoroutine(TiltHead(groundTiltAngle));
            }
            else if (inputVector.x == 0 && lastInput != 0)
            {
                StartCoroutine(TiltHead(0));
            }
            lastInput = inputVector.x;
        }
    }

    //Move input action
   public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    //Mouse control input action
    public void OnLook(InputAction.CallbackContext context)
    {
        //Was using camera rotation, but that had some stutter issues. Rotating whole player seems to work. If issues down the line might have to revert back to adjusting camera.
        cameraMovement = context.ReadValue<Vector2>() * mouseSensitivity;
        Vector3 currentRotation = playerInput.camera.transform.localEulerAngles;

        //Add last frame angle with mousedelta and clamp the vertical. Mathf.clamp won't work with the angles so have to custom clamp it.
        float xRotation = (cameraMovement.y * mouseSensitivity * -1) + transform.localEulerAngles.x;
        if(transform.localEulerAngles.x < 90 && xRotation > lookVerticalMin)
        {
            xRotation = lookVerticalMin;
        }
        else if(transform.localEulerAngles.x > 270 && xRotation < lookVerticalMax)
        {
            xRotation = lookVerticalMax;
        }
        float yRotation = (cameraMovement.x * mouseSensitivity) + transform.localEulerAngles.y;

        //Rotate camera's x and player's y
        transform.localEulerAngles = new Vector3(xRotation, yRotation, transform.localEulerAngles.z);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJump = context.performed;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            if (!isSliding)
            {
                characterController.height = normalHeight * crouchHeightPercent;
                isSliding = true;
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && currentDashTime + dashLength + dashCoolDown < Time.time && !OnWall && !isSliding)
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
            dashParticles.SetActive(true);
        }
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            currentWeapon.UseWeapon();
        }
    }

    //Used to tilt heads for when on a wall or moving on ground if enabled. Currently a little too course as it moves the camera in ints over fixed time. If using ground camera tilt then need to smooth, but on the wall its okay.
    public IEnumerator TiltHead(float angleToTilt)
    {
        int currentTilt = (int)transform.localEulerAngles.z;
        while(currentTilt != angleToTilt && tiltHead)
        {
            if(currentTilt > 300)
            {
                currentTilt++;
            }
            else if(currentTilt < angleToTilt)
            {
                currentTilt++;
            }
            else
            {
                currentTilt--;
            }

            if (currentTilt >= 360)
            {
                currentTilt = 0;
            }

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTilt);
            yield return new WaitForFixedUpdate();
        }
    }

    //A way to get the wish direction with just the camera rotation.
    private void GetDirectionLooking()
    {
        inputVector.y = 1;
        GetWishDirection();
        inputVector.y = 0;
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
}
