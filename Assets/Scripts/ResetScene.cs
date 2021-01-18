using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//For early testing purposes, delete later
public class ResetScene : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
