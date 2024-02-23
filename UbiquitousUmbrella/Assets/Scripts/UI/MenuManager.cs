using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public static bool gameIsPaused;
    //references to canvases
    public GameObject pauseMenu;
    public GameObject PlayerHud;
    public GameObject Inventory;

    // reference to camera controller for cursor lock state
public AudioSource audio;

    void Start()
    {
        //Finds the player hud so it doesn't have to be manually assigned.
        PlayerHud = GameObject.Find("Player_Hud");

        pauseMenu.SetActive(false);
        PlayerHud.SetActive(true);
        Inventory.SetActive(false);
        LockCursor();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { // sets gameIsPaused to bool to false to start
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            gameIsPaused = !gameIsPaused;
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    
    }


    //Checks if static bool gameIsPaused is true and then pauses games
    void PauseGame()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(true);
            PlayerHud.SetActive(false);
            UnlockCursor();
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    //Check if game is already paused then resumes game
    public void ResumeGame()
    {
        if (!gameIsPaused)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            PlayerHud.SetActive(true);
            LockCursor();
        }

    }

    public void OpenInventory()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 1f;
            Inventory.SetActive(true);
            PlayerHud.SetActive(false);
            pauseMenu.SetActive(false);
            UnlockCursor();
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void CloseInventory()
    {
        if (!gameIsPaused)
        {
            Time.timeScale = 1;
            Inventory.SetActive(false);
            pauseMenu.SetActive(false);
            PlayerHud.SetActive(true);
            LockCursor();
        }
    }


    //Goes back to main menu and sets gameIsPaused bool back to false
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
        gameIsPaused = !gameIsPaused;
        audio.Play();
        pauseMenu.SetActive(false);
        PlayerHud.SetActive(true);
    }


    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void ResumeButton()
    {       
        audio.Play();
        gameIsPaused= false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        PlayerHud.SetActive(true);
        LockCursor();
    }
    public void OptionsMenu()
    {
       // audio.Play();
    }
}
