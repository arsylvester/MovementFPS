﻿using System.Collections;
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
    [SerializeField] float wallJumpForce = 1;
    [SerializeField] float wallRunSpeedCap = 1;
    [SerializeField] float slideSpeed = 1;
    [SerializeField] float slideFastLength = 1f;
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
                //Sliding on ground
                if (isSliding)
                {
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
                    if (!firstFrameGrounded)
                    {
                        currentVelocity = ApplyFriction(friction);
                    }
                    currentVelocity = Accelerate(accelerationGround);
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
                    OnWall = true;
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

                    if(wallRight)
                    {
                        //float directionAngle = 270 * Mathf.Deg2Rad;
                        ///currentVelocity.x = ((Mathf.Cos(directionAngle) * hitRight.normal.x) - (Mathf.Sin(directionAngle) * hitRight.normal.y));
                        //currentVelocity.y = ((Mathf.Sin(directionAngle) * hitRight.normal.z) + (Mathf.Cos(directionAngle) * hitRight.normal.z));
                        print("Wall to right.");
                    }
                    else
                    {
                        //currentVelocity.x = hitLeft.normal.z * wallRunSpeed;
                       // currentVelocity.y = hitLeft.normal.x * wallRunSpeed;
                        print("Wall to Left");
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
                        movementVector.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                    }
                }
                else
                {
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
            if (!isSliding)
            {
                characterController.height = normalHeight / 2;
                isSliding = true;
                currentSlideTime = Time.time;
            }
        }
        else
        {
            characterController.height = normalHeight;
            isSliding = false;
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
        }
    }

    private void GetDirectionLooking()
    {
        inputVector.y = 1;
        GetWishDirection();
        inputVector.y = 0;
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
