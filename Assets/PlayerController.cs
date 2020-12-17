using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float maxNormalSpeed = 5;
    [SerializeField] float deceleration = 1;
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float lookVerticalMin = -85;
    [SerializeField] float lookVerticalMax = 85;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.0f;
    Vector2 inputVector;
    Vector3 movementVector;
    Vector2 cameraMovement;
    PlayerInput playerInput;
    CharacterController characterController;
    bool isGrounded;
    bool isJump;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //Check grounded
        isGrounded = characterController.isGrounded;
        if(isGrounded && movementVector.y < 0)
        {
            movementVector.y = 0;
        }

        if(isJump && isGrounded)
        {
            movementVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        //Using theorem x2 = xcosB - ysinB
        //y2 = xsinB + ycosB
        //Unity works clockwise, math typically goes counterclockwise so use recipical? (360 - angle)
        float directionAngle = (360 - transform.eulerAngles.y) * Mathf.Deg2Rad;
        movementVector.x += ((Mathf.Cos(directionAngle) * inputVector.x) - (Mathf.Sin(directionAngle) * inputVector.y)) * speed;
        movementVector.z += ((Mathf.Sin(directionAngle) * inputVector.x) + (Mathf.Cos(directionAngle) * inputVector.y)) * speed;

        movementVector.x = Mathf.Lerp(movementVector.x, 0, deceleration);
        movementVector.z = Mathf.Lerp(movementVector.z, 0, deceleration);

        movementVector.y += gravity * Time.deltaTime;
        characterController.Move(movementVector);
        //Debug.Log("Angle: " + directionAngle * Mathf.Rad2Deg + " x: " + movementVector.x + " z: " + movementVector.z + " Input x:" + inputVector.x + " Input y:" + inputVector.y);
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
}
