using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
  public static bool gameIsPaused = false;
  public static bool inOptions = false;
  public GameObject pauseMeanUI;
  public GameObject optionsMeanUI;
  public GameObject inventoryUI;
  public GameObject HUD;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void onPause()

  {
    if (gameIsPaused && inOptions)
    {
      GoBack();
    }
    else if (gameIsPaused && inOptions != true)
        {
            audioManager.DimGlobalVolume(0.2f);
            Resume();
    }
    else
    {
      Pause();
    }
  }


  public void EnterOptions()
  {
    inOptions = true;
  }
  public void GoBack()
  {
    optionsMeanUI.SetActive(false);
    pauseMeanUI.SetActive(true);
    inOptions = false;
  }

  public void Resume()
    {
        audioManager.DimGlobalVolume(1);
        HUD.SetActive(true);
    pauseMeanUI.SetActive(false);
    Time.timeScale = 1f;
    gameIsPaused = false;
  }

  void Pause()
    {
        audioManager.DimGlobalVolume(0.1f);
        audioManager.PauseWalkSound();
        HUD.SetActive(false);
    pauseMeanUI.SetActive(true);
    Time.timeScale = 0f;
    gameIsPaused = true;
  }

  public void LoadMenu()
  {
    gameIsPaused = false;
    Time.timeScale = 1f;
    SceneManager.LoadScene("Main Menu");
  }

  public void QuitGame()
  {
    Debug.Log("QUIT!");
    Application.Quit();
  }

}