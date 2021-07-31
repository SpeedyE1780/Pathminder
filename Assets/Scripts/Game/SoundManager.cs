using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource BackgroundMusic;
    public AudioSource MoveSound;
    public AudioSource RespawnSound;
    public AudioSource LevelWon;
    public AudioSource ButtonClicked;
    
    private void Awake()
    {
        Instance = this;

        //Set the background music
        BackgroundMusic.Play();
        BackgroundMusic.loop = true;
        BackgroundMusic.volume = 0.3f;
    }


    public void ToggleSFX()
    {
        MoveSound.mute = !MoveSound.mute;
        RespawnSound.mute = !RespawnSound.mute;
        LevelWon.mute = !LevelWon.mute;
        ButtonClicked.mute = !ButtonClicked.mute;
        UIManager.Instance.ToggleSFXIcon();
    }

    public void ToggleMusic()
    {
        BackgroundMusic.mute = !BackgroundMusic.mute;
        UIManager.Instance.ToggleMusicIcon();
    }
}
