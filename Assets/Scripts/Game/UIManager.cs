using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text LevelText;
    public GameObject MainMenuUI;
    public GameObject GameMenuUI;
    public GameObject PauseMenuUI;
    public GameObject NextLevelMenuUI;
    public GameObject LoadingScreen;
    public Image MusicIcon;
    public Image PMusicIcon;
    public Sprite MusicSprite;
    public Sprite MutedMusicSprite;
    public Image SFXIcon;
    public Image PSFXIcon;
    public Sprite SFXSprite;
    public Sprite MutedSFXSSprite;


    private void Awake()
    {
        Instance = this;
    }



    private void OnEnable()
    {
        EventManager.nextLevel += ShowNextLevelMenu;
    }



    private void OnDisable()
    {
        EventManager.nextLevel -= ShowNextLevelMenu;
    }

    //Change the text
    public void UpdateText(int Level)
    {
        LevelText.text = "Level: " + (Level + 1);
    }

    //Turns the menu on and off and changes canPause to the opposite of the menu
    public void ToggleMenu(ref bool canPause , int Delay = 0)
    {
        if (Delay == 0)
        {
            MainMenuUI.SetActive(!MainMenuUI.activeSelf);
            GameMenuUI.SetActive(!GameMenuUI.activeSelf);
            canPause = GameMenuUI.activeSelf;
        }
        else
        {
            Invoke("DelayedToggle", Delay);
            canPause = !GameMenuUI.activeSelf;
        }

    }

    void DelayedToggle()
    {
        MainMenuUI.SetActive(!MainMenuUI.activeSelf);
        GameMenuUI.SetActive(!GameMenuUI.activeSelf);
    }


    //Go to loading screen
    public void ActivateLoading()
    {
        LoadingScreen.SetActive(true);
        LoadingScreen.GetComponent<NewLoadingFade>().FadeIn();
    }


    void ShowNextLevelMenu()
    {
        Invoke("EndofLevelMenu", 1.5f);
    }
    void EndofLevelMenu()
    {
        NextLevelMenuUI.GetComponent<NewFade>().FadeIn();
    }


    public void ShowPauseMenu()
    {
        PauseMenuUI.GetComponent<NewFade>().FadeIn();
    }

    public void ToggleMusicIcon()
    {
        if(MusicIcon.sprite == MusicSprite)
        {
            MusicIcon.sprite = MutedMusicSprite;
            PMusicIcon.sprite = MutedMusicSprite;
        }
        else
        {
            MusicIcon.sprite = MusicSprite;
            PMusicIcon.sprite = MusicSprite;
        }
    }



    public void ToggleSFXIcon()
    {
        if (SFXIcon.sprite == SFXSprite)
        {
            SFXIcon.sprite = MutedSFXSSprite;
            PSFXIcon.sprite = MutedSFXSSprite;
        }
        else
        {
            SFXIcon.sprite = SFXSprite;
            PSFXIcon.sprite = SFXSprite;
        }
    }
}
