using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region P U B L I C    V A R I A B L E S
    public static GameManager Instance;
    public RectTransform Background;
    public Image GameBackground;
    public Transform GridCell;
    public Grid GridManager;
    public Transform Characters;
    public Transform HiddenEnemy;
    public Transform Player;
    public Transform Enemy;
    public Transform Pursuer;
    public Transform ShadowPlayer;
    #endregion

    #region P R I V A T E    V A R I A B L E S
    bool pursuerSpawned;
    int level;
    float screenWidth;
    float offset;
    float side; //Side of each cell
    bool canPause; //Checks if the game can be paused
    bool isPaused; //Checks if the game is paused
    bool canReplay; //Checks if the game can be replayed
    bool isReplaying; //Checks if the game is replaying
    bool activateMenu; //Checks if the Destroy() toggle the menu
    string PresetPath; //Path to load the preset from
    EditorPresetList PresetList;
    #endregion

    #region S T A R T || O N  E N A B L E || O N  D I S A B L E

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the Game Manager Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        
        isPaused = false;
        activateMenu = false;

        //Instantiate the players and the enemies
        InstantiateGameObject();

        //Set the path
        PresetPath = "LevelTemplates";

        //Load Game Data
        LoadGame();


        //Generate a random color for the background
        ShuffleBackground();
    }

    private void OnEnable()
    {
        EventManager.respawn += StartRespawn;
        EventManager.nextLevel += NextLevel;
        EventManager.spawn += SpawnEnemy;
        EventManager.enableControls += EnableButtons;
        
    }

    private void OnDisable()
    {
        EventManager.respawn -= StartRespawn;
        EventManager.nextLevel -= NextLevel;
        EventManager.spawn -= SpawnEnemy;
        EventManager.enableControls -= EnableButtons;
    }

    #endregion

    #region  F U N C T I O N S  |  C O R O U T I N E S

    // LEVEL BUILDING
    // -------------------------------------------

    void StartGame()
    {
        //Deactivate the main menu and destroy the menu level
        UIManager.Instance.ToggleMenu(ref canPause , 1);
        //Activate the loading screen and build the level
        UIManager.Instance.ActivateLoading();
        Invoke("BuildLevel", 0.5f);
    }

    //Give a new color to the background
    void ShuffleBackground()
    {
        Vector4 randomColor = Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
        GameBackground.color = randomColor;

        float playerHue;
        float playerSaturation;
        float playerValue;
        Color.RGBToHSV(GameBackground.color, out playerHue, out playerSaturation, out playerValue);

        if (playerHue + 0.3f > 1)
        {
            playerHue -= 0.3f;
        }
        else
        {
            playerHue += 0.3f;
        }

        Player.GetComponent<Image>().color = Color.HSVToRGB(playerHue, playerSaturation, playerValue);
    }


    void BuildLevel()
    {
        //Scale the gameObjects
        ScaleGameObjects();

        GridManager.DrawGrid(PresetList.Levels[level] , offset);

        //Initialize the player
        Player.gameObject.SetActive(true);
        Player.GetComponent<PlayerController>().InitializePlayer();

        UIManager.Instance.UpdateText(level);

        //If is replaying show the enemies 
        if (isReplaying)
        {
            Invoke("UnhideEnemies", 0.5f);
            isReplaying = false;
        }

        //Disable Game Buttons
        canPause = false;
        canReplay = false;

        Invoke("SpawnShadow", 0.75f);
    }

    //Function to make the grid occupy the full screen
    void ScaleGameObjects()
    {
        //Distance between cells
        int offsetCell = 2;

        //Get the Width/Height from the Background
        screenWidth = Background.rect.width;

        //Get the Side by dividing screen width minus the offset between cells by gridSizeX
        side = (screenWidth * 0.8f - (PresetList.Levels[level].GridSizeX - 1) * offsetCell) / PresetList.Levels[level].GridSizeX;

        PresetList.Levels[level].Side = side;

        offset = screenWidth - screenWidth * 0.8f;
        offset *= 0.5f;
        

        //Scale the gameobjects
        GridCell.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(side, side);
        Player.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(side, side);
        Enemy.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(side, side);
        Pursuer.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(side, side);
    }



    //Delay destroying the level to wait for the loading screen to fade in
    void DelayDestroy(float Delay = 0.5f)
    {
        //Stop the pursuer from moving
        Pursuer.GetComponent<PursuerController>().StopAllCoroutines();
        Invoke("DestroyLevel", Delay);
    }

    //Destroy the grid and the enemies and player
    void DestroyLevel()
    {
        //Stop the pursuer from spawning in case he was spawned
        CancelInvoke();

        //Destroy game objects
        GridManager.DestroyGrid();
        pursuerSpawned = false;
        Pursuer.GetComponent<PursuerController>().Destroy();
        Player.gameObject.SetActive(false);
        ShadowPlayer.gameObject.SetActive(false);

        //Destroy Enemies
        foreach (Transform t in HiddenEnemy)
        {
            Destroy(t.gameObject);
        }


        //activate the menu
        if (activateMenu)
        {
            UIManager.Instance.ToggleMenu(ref canPause);
            activateMenu = false;
        }

        //If Menu is inactive we build a level
        if (!UIManager.Instance.MainMenuUI.activeInHierarchy)
        {
            BuildLevel();
        }
    }

    //Destroy the current level and build the next level
    public void GotoNextLevel()
    {
        //Change background color
        Invoke("ShuffleBackground", 0.45f);

        DelayDestroy();

        //Activate the loading screen
        UIManager.Instance.ActivateLoading();
    }

    // ------------------------------------------


    // PLAYER || ENEMIES FUNCTIONS
    //----------------------------------------------------------------------

    //Instantiate gameobjects
    void InstantiateGameObject()
    {
        Player = Instantiate(Player, Characters);
        Player.gameObject.SetActive(false);

        Pursuer = Instantiate(Pursuer, Characters);
        Pursuer.gameObject.SetActive(false);

        ShadowPlayer = Instantiate(ShadowPlayer, Characters);
        ShadowPlayer.gameObject.SetActive(false);
    }


    //Spawn Shadow player
    void SpawnShadow()
    {
        ShadowPlayer.GetComponent<ShadowPlayerController>().InitializeShadow(PresetList.Levels[level].Path , PresetList.Levels[level].Side , level , GameBackground.color);
    }


    //Move the player when they click on a grid cell
    public void MovePlayer(Transform TargetCell)
    {
        //Check if the game is paused or no
        if(!isPaused)
        {
            //Move the player
            Player.GetComponent<PlayerController>().MovePlayer(TargetCell);

            //Spawns a pursuer 5 seconds after the player's first move enable replaying after the player's first move
            if (!pursuerSpawned && Player.GetComponent<PlayerController>().Moved)
            {
                pursuerSpawned = true;
                Invoke("SpawnPursuer", 3);
            }
        }
    }

    // Create Enemy
    void SpawnPursuer()
    {
        Pursuer.GetComponent<PursuerController>().Spawn(Player.GetComponent<PlayerController>());
    }

    //Spawns Hidden Enemy
    public void SpawnEnemy(Vector2 Pos)
    {
        GameObject e = Instantiate(Enemy.gameObject, HiddenEnemy);
        e.GetComponent<Image>().rectTransform.anchoredPosition = Pos;
        e.GetComponent<Image>().DOFade(1, 0.5f);
    }

    //Unhide the enemies
    void UnhideEnemies()
    {
        HiddenEnemy.gameObject.SetActive(true);
    }

    //------------------------------------------------------------------


    //R E S P A W N
    //------------------

    // Respawn
    void StartRespawn()
    {
        //Cancel Invoke in case the pursuer wasn't spawned stop the pursuer in case he was spawned
        CancelInvoke();

        //Play respawn sound
        SoundManager.Instance.RespawnSound.Play();

        pursuerSpawned = false;
        Pursuer.GetComponent<PursuerController>().StopAllCoroutines();
        canReplay = false;
        canPause = false;
        Invoke("Respawn", 1);
    }
    void Respawn()
    {
        //Destroy the pursuer
        Pursuer.GetComponent<PursuerController>().Destroy();
    }
    
    //----------------------------------------------------------



    //P O P  U P  M E N U E S || L O A D I N G  S C R E E N
    //----------------------------------------------------------------------------------------------------------------
    
    // Open the next level menu
    void NextLevel()
    {
        //Cancel Invoke in case the pursuer wasn't spawned stop the pursuer in case he was spawned
        CancelInvoke();
        Pursuer.GetComponent<PursuerController>().StopAllCoroutines();


        //Disable from pausing or replaying when the level is complete
        canPause = false;
        canReplay = false;

        //Level up
        //Increment the level and Save the new level value
        level++;

        //Repeat level once all levels are completed
        if(level >= PresetList.Levels.Length)
        {
            level = 0;
        }

        //Play level won sound
        SoundManager.Instance.LevelWon.time = 1.6f;
        SoundManager.Instance.LevelWon.Play();

        SaveGame();
    }


    //Destroy the current level and go to the main menu
    public void GotoMainMenu()
    {
        DelayDestroy();

        UIManager.Instance.ActivateLoading();
        
        //activate the menu stop the game from being paused 
        activateMenu = true;
        isPaused = false;
        Time.timeScale = 1;
    }


    //----------------------------------------------------------------------------



    //P A U S E || R E P L A Y  G A M E
    //-------------------------------------------------------

    //Destroy current level and build it again
    public void ReplayGame()
    {
        if(canReplay)
        {
            DelayDestroy(0);
            HiddenEnemy.gameObject.SetActive(false);
            isReplaying = true;
        }
    }
  
    //Set the time scale to 0 pause the game
    public void PauseGame()
    {
        //Checks if the game can be paused
        if(canPause)
        {
            //Make sure the game isn't already paused
            if (!isPaused)
            {
                isPaused = true;
                Time.timeScale = 0;
                UIManager.Instance.ShowPauseMenu();
            }
        }
    }

    //Resume the game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }


    //Enables Pausing/Replaying
    void EnableButtons()
    {
        canPause = true;
        canReplay = true;
    }


    //-----------------------------------------------------------------



    //S A V E || L O A D  G A M E
    //--------------------------------

    void SaveGame()
    {
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.Save();
    }

    void LoadGame()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level");

            UIManager.Instance.UpdateText(level);
        }
        else
        {
            PlayerPrefs.SetInt("Level", 0);
            level = 0;
            UIManager.Instance.UpdateText(level);
        }

        //Load the level templates as text then read it
        TextAsset jsonData = Resources.Load<TextAsset>(PresetPath);
        PresetList = JsonUtility.FromJson<EditorPresetList>(jsonData.ToString());
    }

    //------------------------------------------------------------------

    #endregion
}