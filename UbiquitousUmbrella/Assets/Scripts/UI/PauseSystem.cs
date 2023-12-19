using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour
{
    public static bool gameIsPaused;
    public GameObject pauseMenu;
    public GameObject PlayerHud;


    void Start()
    {
        pauseMenu.SetActive(false);
        PlayerHud.SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }

        else
        {
            ResumeGame();
        }
    }
    void PauseGame()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(true);
            PlayerHud.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ResumeGame()
    {
        if (!gameIsPaused)
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            PlayerHud.SetActive(true);
        }

    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
