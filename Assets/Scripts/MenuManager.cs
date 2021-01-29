using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeReference] GameObject options;

    public void OnSceneStart(string sceneToMove)
    {
        SceneManager.LoadScene(sceneToMove);
    }

    public void ShowOptions()
    {
        menu.SetActive(false);
        options.SetActive(true);
    }
    public void ReturnToMenu()
    {
        menu.SetActive(true);
        options.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
