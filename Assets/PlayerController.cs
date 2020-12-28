using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   // [SerializeField] float speed = 1;
    [SerializeField] float accelerationAir = 5;
    [SerializeField] float accelerationGround = 5;
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float friction = 1;
    [SerializeField] float airResistance = 1;
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float lookVerticalMin = -85;
    [SerializeField] float lookVerticalMax = 85;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float dashLength = 1.0f;
    [SerializeField] float dashCoolDown = 1.0f;
    [SerializeField] float DashVelocity = 5f;
    Vector2 inputVector;
    Vector2 wishDirection;
    Vector3 movementVector;
    Vector2 cameraMovement;
    Vector2 currentVelocity;
    PlayerInput playerInput;
    CharacterController characterController;
    bool isGrounded;
    bool isJump;
    bool firstFrameGrounded = true;
    bool wallRight;
    float normalHeight;
    float currentDashTime;

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

        if(isJump && isGrounded)
        {
            movementVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            firstFrameGrounded = true;
        }

        //Not Dashing
        if (currentDashTime + dashLength < Time.time)
        {
            GetWishDirection();

            currentVelocity.x = movementVector.x;
            currentVelocity.y = movementVector.z;

            if (isGrounded)
            {
                if (!firstFrameGrounded)
                {
                    currentVelocity = ApplyFriction(friction);
                }
                currentVelocity = Accelerate(accelerationGround);
                firstFrameGrounded = false;
                movementVector.y += gravity * Time.fixedDeltaTime;
            }
            else
            {
                //Wall running
                if ((characterController.collisionFlags & CollisionFlags.CollidedSides) != 0)
                {
                    print(characterController.velocity);
                    movementVector.y = 0;

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
                    else
                    {
                        wallRight = false;
                    }

                    if(wallRight)
                    {
                        print("Wall to right.");
                    }
                    else
                    {
                        print("Wall to Left");
                    }

                    if(isJump)
                    {
                        if(wallRight)
                        {
                            currentVelocity = characterController.velocity.normalized + new Vector3(-1, -1, 0);
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
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
        //Trying chaning this to always be in the wishdirection, but now always caps at max velocity, and stops too abrubtly. 
        if (wishDirection.magnitude == 0)
        {
            return ApplyFriction(airResistance);
        }
        else
        {
            return currentVelocity + wishDirection.normalized * acceleratedVelocity;
        }
    }

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

   public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

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
       // playerInput.camera.transform.localEulerAngles = new Vector3(xRotation, 0, 0);
        transform.localEulerAngles = new Vector3(xRotation, yRotation, 0);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJump = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            characterController.height = normalHeight / 2;
        }
        else
        {
            characterController.height = normalHeight;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && currentDashTime + dashLength + dashCoolDown < Time.time)
        {
            currentDashTime = Time.time;
            //If no direction dash forward.
            if(inputVector.magnitude == 0)
            {
                inputVector.y = 1;
                GetWishDirection();
                inputVector.y = 0;
            }
            else
            {
                GetWishDirection();
            }
            movementVector.y = 0;
        }
    }

    public float GetCurrentSpeed()
    {
        return currentVelocity.magnitude;
    }

    public float GetDashPercent()
    {
        return  (Time.time - currentDashTime) / (dashLength + dashCoolDown);
    }
}
