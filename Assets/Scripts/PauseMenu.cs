using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject options;
    [SerializeField] GameObject pause;
    PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pause.SetActive(!pause.activeInHierarchy);
        Cursor.visible = pause.activeInHierarchy;
        if (pause.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            player.StopMovement();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            player.ResumeMovement();
        }
    }

    public void OnSceneStart(string sceneToMove)
    {
        TogglePauseMenu();
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
