using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject Screen1;
    [SerializeField] private GameObject Screen2;
    [SerializeField] private AudioManager audioManager;

    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoScreen1()
    {
        mainMenu.SetActive(false);
        Screen1.SetActive(true);
        audioManager.PlayGurtSounds();
    }

    public void GoScreen2()
    {
        Screen1.SetActive(false);
        Screen2.SetActive(true);
    }

    public void Translate()
    {
        Debug.Log("Translate");
        audioManager.OnGurtAussieSounds(Vector3.zero);
        audioManager.MuteGurtSounds();
    }

    public void QuitGame ()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

}
