using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
    { 
        public static bool gameIsPaused;
        public GameObject pauseMenu;

    void Start ()
    {
        pauseMenu.SetActive(false);
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
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        void ResumeGame()
        {
            if (!gameIsPaused)
            {
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
            }
        }
    }
