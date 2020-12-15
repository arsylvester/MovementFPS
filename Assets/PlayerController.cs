using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float mouseSensitivity = 1;
    [SerializeField] float lookVerticalMin = -85;
    [SerializeField] float lookVerticalMax = 85;
    float horizontal;
    float vertical;
    Vector2 cameraMovement;
    PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(horizontal, 0, vertical);
    }

   public void OnMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x * speed * .1f;
        vertical = context.ReadValue<Vector2>().y * speed * .1f;
        //Debug.Log(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        cameraMovement = context.ReadValue<Vector2>() * mouseSensitivity;
        Vector3 currentRotation = playerInput.camera.transform.localEulerAngles;

        //Add last frame angle with mousedelta and clamp the vertical. Mathf.clamp won't work with the angles so have to custom clamp it.
        float xRotation = (cameraMovement.y * mouseSensitivity * -1) + currentRotation.x;
        if(currentRotation.x < 90 && xRotation > lookVerticalMin)
        {
            xRotation = lookVerticalMin;
        }
        else if(currentRotation.x > 270 && xRotation < lookVerticalMax)
        {
            xRotation = lookVerticalMax;
        }
        float yRotation = (cameraMovement.x * mouseSensitivity) + currentRotation.y;

        playerInput.camera.transform.localEulerAngles = new Vector3(xRotation, yRotation, 0);
    }
}
