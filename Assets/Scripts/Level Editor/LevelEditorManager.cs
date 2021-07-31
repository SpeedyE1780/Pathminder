using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class LevelEditorManager : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S
    public static LevelEditorManager Instance;
    public GameObject MenuUI;
    public GameObject EditorUI;
    public InputField LevelInput;
    public InputField GridInput;
    public Text EditLevel;
    public GridManager gridManager;
    public Transform EnemyParent;
    public GameObject Enemy;
    #endregion

    #region P R I V A T E  V A R I A B L E S
    private int level;
    private int gridSizeX;
    private bool addPath;
    private List<Vector2Int> Path;
    private EditorCell currentCell;
    private bool addEnemy;
    private List<Vector2Int> EnemyPositions;
    private EditorPresetList levelTemplates;
    private string saveDirectory;
    private string SavePath;
    private string templatesPath;
    #endregion

    private void Start()
    {
        Instance = this;
        addPath = false;
        addEnemy = false;
        Path = new List<Vector2Int>();
        EnemyPositions = new List<Vector2Int>();

        saveDirectory = Application.dataPath + "/Resources" + "/Levels";
        SavePath = saveDirectory+ "/Level";
        templatesPath = Application.dataPath + "/Resources" + "/LevelTemplates.json";
    }

    #region F U N C T I O N S

    //Menu Buttons
    //Create/Load Presets
    //----------------------------

    //Create an empty preset
    public void CreatePreset()
    {
        InitializeValues();
        ToggleMenu();
        gridManager.DrawGrid(gridSizeX);
    }


    //Load a prebuilt preset
    public void LoadPresets()
    {
        InitializeValues();
        ToggleMenu();

        string levelPath = SavePath + (level + ".json");

        //LoadPreset
        if (File.Exists(levelPath))
        {
            EditorPreset Temp = JsonUtility.FromJson<EditorPreset>(File.ReadAllText(levelPath));

            //Initialize the values
            Path = Temp.Path;
            EnemyPositions = Temp.EnemyPositions;
            gridSizeX = Temp.GridSizeX;

            gridManager.DrawPreset(Temp);

            Vector2Int lastPos = Temp.Path[Path.Count - 1];
            currentCell = gridManager.GridArray[lastPos.x, lastPos.y].GetComponent<EditorCell>();

            foreach (Vector2Int E in Temp.EnemyPositions)
            {
                EditorCell Target = gridManager.GridArray[E.x, E.y].GetComponent<EditorCell>();

                Target.Clicked = true;
                //Update the visuals
                Target.GetComponent<Image>().color = Color.gray;
                GameObject e = Instantiate(Enemy, EnemyParent);
                e.GetComponent<Image>().rectTransform.anchoredPosition = Target.Postion;

                //Add a function to enemy click
                e.GetComponent<Button>().onClick.AddListener(() => RemoveEnemy(e.GetComponent<EditorEnemy>()));
                e.GetComponent<EditorEnemy>().currentCell = Target;
            }
        }

        //Load Empty Grid
        else
        {
            gridManager.DrawGrid(gridSizeX);
        }
    }


    //Get the input field values
    void InitializeValues()
    {
        //Get gridSizeX minimum value 5 maximum value 12
        gridSizeX = int.Parse(GridInput.text);
        if (gridSizeX < 5)
        {
            gridSizeX = 5;
        }
        if (gridSizeX > 12)
        {
            gridSizeX = 12;
        }

        //Get the level minimum value is 0
        level = int.Parse(LevelInput.text);
        if (level < 0)
        {
            level = 0;
        }

        EditLevel.text = "Level: " + level;
    }

    void ToggleMenu()
    {
        MenuUI.SetActive(!MenuUI.activeSelf);
        EditorUI.SetActive(!EditorUI.activeSelf);
    }

    //----------------------------


    //Editing Menu
    //Path - Enemy - Reset - Save - Quit - AddCell
    //--------------------------------------------

    //Path Button
    //Enable adding cells to Path
    public void ActivatePath()
    {
        addPath = true;
        addEnemy = false;
    }


    //Enemy Button
    //Enable adding cells to EnemyPosition
    public void ActivateEnemy()
    {
        addEnemy = true;
        addPath = false;
    }


    //Reset Button
    //Reset the Grid
    public void ResetGrid()
    {
        ResetValues();

        gridManager.DestroyGrid();
        gridManager.DrawGrid(gridSizeX);

    }


    //Save Button
    //Save the current level template
    public void SaveTemplate()
    {
        ToggleMenu();

        EditorPreset Temp = new EditorPreset(level, Path, EnemyPositions, gridManager.GridSizeX, gridManager.GridSizeY, gridManager.Side);

        string levelPath = SavePath + (Temp.Level.ToString() + ".json");

        File.WriteAllText(levelPath, JsonUtility.ToJson(Temp , true));

        SavePresets();

        ResetValues();
        gridManager.DestroyGrid();
    }

    //Save All the Presets
    public void SavePresets()
    {
        //Get all the Levels json files
        DirectoryInfo AllLevels = new DirectoryInfo(saveDirectory);
        FileInfo[] Level = AllLevels.GetFiles("*.json");

        //Initialize the array to the number of levels
        levelTemplates = new EditorPresetList(Level.Length);

        //Add all the levels templates to the list
        foreach (FileInfo f in Level)
        {
            //Load the level and place it correctly in the array
            EditorPreset Temp = JsonUtility.FromJson<EditorPreset>(File.ReadAllText(f.FullName));
            levelTemplates.Levels[Temp.Level] = Temp;
        }
        //Save all the templates to a file
        File.WriteAllText(templatesPath, JsonUtility.ToJson(levelTemplates, true));
    }


    //Quit Button
    //Stop Editing go to the menu
    public void CancelEdit()
    {
        ToggleMenu();
        ResetValues();
        gridManager.DestroyGrid();
    }


    //Reset the values
    void ResetValues()
    {
        addPath = false;
        addEnemy = false;
        Path = new List<Vector2Int>();
        EnemyPositions = new List<Vector2Int>();
        currentCell = null;

        foreach (Transform E in EnemyParent)
        {
            Destroy(E.gameObject);
        }
    }


    //-----------------------------------------------

    //Grid Editing
    //-------------------

    //Add/Remove Cells to the path or enemy positions
    public void AddCell(EditorCell Target)
    {
        //Cell has already been clicked
        if(Target.Clicked)
        {
            //Check that the cell belongs to the path
            if(Path.Contains(Target.GridPos))
            {
                //Check that it's the last cell on the path
                if (Path.IndexOf(Target.GridPos) == (Path.Count - 1))
                {

                    //Delete the cell from the path and make it back to unclicked
                    Path.Remove(Target.GridPos);
                    Target.GetComponent<Image>().color = Color.black;
                    Target.Clicked = false;

                    //Set the current cell to the last cell in the path
                    if (Path.Count > 0)
                    {
                        currentCell = gridManager.GridArray[Path[Path.Count - 1].x, Path[Path.Count - 1].y].GetComponent<EditorCell>();
                        currentCell.GetComponent<Image>().color = Color.red;
                    }

                    //Set the current cell to null
                    else
                    {
                        currentCell = null;
                    }
                } 
            }
        }

        else
        {
            //Adding cells to the paths
            if (addPath)
            {
                //Path is empty we add the start position and update the current cell
                if (Path.Count == 0)
                {
                    Path.Add(Target.GridPos);
                    //Change the color to red to know the current cell
                    Target.GetComponent<Image>().color = Color.red;
                    currentCell = Target;

                    //Update the cell to a clicked cell
                    Target.Clicked = true;
                }
                else
                {
                    //Check if the Target is a neighbor 
                    if (currentCell.Neighbors.Contains(Target))
                    {
                        //Add its position to the path and update current cell
                        Path.Add(Target.GridPos);

                        //Change the color of the previous cell to gray
                        currentCell.GetComponent<Image>().color = Color.gray;
                        currentCell = Target;

                        //Change the color to red to know the current cell
                        currentCell.GetComponent<Image>().color = Color.red;

                        //Update the cell to a clicked cell
                        Target.Clicked = true;
                    }
                }
            }

            //Adding Enemies
            if (addEnemy)
            {
                //Add the position to the Enemy Position List
                EnemyPositions.Add(Target.GridPos);

                //Update the cell to a clicked cell
                Target.Clicked = true;

                //Update the visuals
                Target.GetComponent<Image>().color = Color.gray;
                GameObject e = Instantiate(Enemy, EnemyParent);
                e.GetComponent<Image>().rectTransform.anchoredPosition = Target.Postion;

                //Add a function to enemy click
                e.GetComponent<Button>().onClick.AddListener(() => RemoveEnemy(e.GetComponent<EditorEnemy>()));
                e.GetComponent<EditorEnemy>().currentCell = Target;
            }
        }
    }


    //Delete enemy from the preset
    public void RemoveEnemy(EditorEnemy enemy)
    {
        enemy.currentCell.Clicked = false;
        enemy.currentCell.GetComponent<Image>().color = Color.black;
        EnemyPositions.Remove(enemy.currentCell.GridPos);
        Destroy(enemy.gameObject);
    }

    //----------------------------------------------------------------

    #endregion
}
