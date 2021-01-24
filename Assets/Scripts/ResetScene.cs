using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//For early testing purposes, delete later
public class ResetScene : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] bool resetScene = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            if (resetScene)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                CheckpointSystem.instance.ResetPlayerToCheckpoint();
            }
        }
    }

    private void Update()
    {
        if(Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
