using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PauseMenu : MonoBehaviour
{

    private bool PauseGame;
    public GameObject PauseGameMenu;

    private GameObject _player;
    
    [SerializeField]
    private GameObject OptionsMenu;

    bool _openOptions = false;


    // Update is called once per frame
    void Update()
    {
        if (OptionsMenu.activeInHierarchy)
        {
            _openOptions = true;
            Time.timeScale = 0.0f;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OptionsMenu.SetActive(false);
                PauseGameMenu.SetActive(true);
                Time.timeScale = 0f;
                PauseGame = true;
            }
            else
            {

                PauseGame = true;
                Time.timeScale = 0f;
            }
        }
        else { _openOptions = false; }
       
        if (Input.GetKeyDown(KeyCode.Escape) && !_openOptions)
        {
            if (PauseGame)
            {
                Time.timeScale = 1f;
                Resume();
            }
            else
            {
                Time.timeScale = 0f;
                Pause();
            }
        }
    }


    public void ExitGame()
    {
        Application.Quit();

    }


    public void Resume()
    {
        Time.timeScale = 1.0f;
        PauseGameMenu.SetActive(false);      
        PauseGame = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        PauseGameMenu.SetActive(true);
        PauseGame = true;
    }


}