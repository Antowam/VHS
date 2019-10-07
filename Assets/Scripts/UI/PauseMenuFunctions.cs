using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuFunctions : MonoBehaviour
{
    public static bool bGameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionsValuesUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(bGameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionsValuesUI.SetActive(false);
        Time.timeScale = 1f;
        bGameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        bGameIsPaused = true;
    }
}
