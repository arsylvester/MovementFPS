using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 1;
    float horizontal;
    float vertical;

    // Start is called before the first frame update
    void Start()
    {
        
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
        Debug.Log(context.ReadValue<Vector2>());
    }
}
