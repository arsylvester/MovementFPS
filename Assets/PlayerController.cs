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
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float lookVerticalMin = -85;
    [SerializeField] float lookVerticalMax = 85;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.0f;
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
    float normalHeight;

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

        //Acceleration caluclations
        //Mostly implemented from my own work but https://adrianb.io/2015/02/14/bunnyhop.html definitly helped, and of course Quake 3.
        GetWishDirection();

        currentVelocity.x = movementVector.x;
        currentVelocity.y = movementVector.z;

        if (isGrounded)
        {

            //Friction
            float speed = currentVelocity.magnitude;
            if(speed != 0 && !firstFrameGrounded)
            {
                float slow = speed * friction * Time.fixedDeltaTime;
                currentVelocity *= Mathf.Max(speed - slow, 0) / speed;
                print("Applying friction");
            }

            if (firstFrameGrounded)
            {
                firstFrameGrounded = false;
            }

            currentVelocity = Accelerate(accelerationGround);
        }
        else
        {
            currentVelocity = Accelerate(accelerationAir);
        }

        movementVector.x = currentVelocity.x;
        movementVector.z = currentVelocity.y;
        movementVector.y += gravity * Time.fixedDeltaTime;

        //print("Current Speed: " + currentVelocity.magnitude);
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

    private Vector2 Accelerate(float acceleration)
    {
        float projectedVelocity = Vector2.Dot(currentVelocity, wishDirection.normalized);
        float acceleratedVelocity = acceleration * Time.fixedDeltaTime;

        if (acceleratedVelocity + projectedVelocity > maxVelocity)
        {
            acceleratedVelocity = maxVelocity - projectedVelocity;
        }
        return currentVelocity + wishDirection.normalized * acceleratedVelocity;
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
        Debug.Log(isJump);
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

    public float GetCurrentSpeed()
    {
        return currentVelocity.magnitude;
    }
}
