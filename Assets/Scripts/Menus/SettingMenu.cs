using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
public class SettingMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
  public void setVolume (float volume)
  {
    audioMixer.SetFloat("volume", volume);
  }

  public void SetFullscreen (bool isFullscreen)
  {
    Screen.fullScreen = isFullscreen;
  }

}
