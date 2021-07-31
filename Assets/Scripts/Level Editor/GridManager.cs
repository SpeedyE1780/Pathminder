using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    #region P U B L I C  V A R I A B L E S

    public static GridManager Instance;
    public Transform editorCell;
    public RectTransform Background;
    public Transform[,] GridArray;
    public int GridSizeX, GridSizeY;
    public float Side;

    #endregion

    #region P R I V A T E  V A R I A B L E S

    float screenWidth, screenHeight;
    float offset;
    Vector2 startPosition;

    #endregion

    #region F U N C T I O N S

    public void DrawGrid(int sizeX)
    {
        Instance = this;

        GridSizeX = sizeX;
        InitializeValues();
        GridArray = new Transform[GridSizeX, GridSizeY];

        Vector2 currentPosition = startPosition;
        for (int y = 0; y < GridSizeY; y++) 
        {
            for (int x = 0; x < GridSizeX; x++) 
            {
                //Instantiate the cell and place it correctly on the screen
                Transform g = Instantiate(editorCell, this.transform);
                g.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y);

                //Add an event listener on click
                g.GetComponent<Button>().onClick.AddListener(() => LevelEditorManager.Instance.AddCell(g.GetComponent<EditorCell>()));

                //Sets the cell position in the grid
                g.GetComponent<EditorCell>().SetXY(x, y, currentPosition);
                GridArray[x, y] = g;

                //Move the current position by the Side + spacing on the x axis
                currentPosition = new Vector2(currentPosition.x + Side + offset, currentPosition.y);
            }

            //Move the current position by the Side + spacing on the y axis and reset the x to the starting x
            currentPosition = new Vector2(startPosition.x, currentPosition.y + Side + offset);
        }

        //Get Neighbors
        foreach (Transform Cell in GridArray)
        {
            Cell.GetComponent<EditorCell>().GetNeighbors();
        }
    }

    public void DrawPreset(EditorPreset preset)
    {
        Instance = this;

        //Initialize Grid Array and Side
        GridArray = new Transform[preset.GridSizeX, preset.GridSizeY];
        GridSizeX = preset.GridSizeX;
        GridSizeY = preset.GridSizeY;
        Side = preset.Side;
        
        //Distance between cells
        offset = 2;

        //Scale the objects
        ScaleObjects();

        Vector2 currentPosition = startPosition;
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                //Instantiate the cell and place it correctly on the screen
                Transform g = Instantiate(editorCell, this.transform);
                g.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y);

                //Add an event listener on click
                g.GetComponent<Button>().onClick.AddListener(() => LevelEditorManager.Instance.AddCell(g.GetComponent<EditorCell>()));

                //Sets the cell position in the grid
                g.GetComponent<EditorCell>().SetXY(x, y, currentPosition);

                GridArray[x, y] = g;

                //Move the current position by the Side + spacing on the x axis
                currentPosition = new Vector2(currentPosition.x + Side + offset, currentPosition.y);
            }

            //Move the current position by the Side + spacing on the y axis and reset the x to the starting x
            currentPosition = new Vector2(startPosition.x, currentPosition.y + Side + offset);
        }

        //Get Neighbors
        foreach (Transform Cell in GridArray)
        {
            Cell.GetComponent<EditorCell>().GetNeighbors();
        }

        //Change the cells to clicked and change their color
        foreach(Vector2Int P in preset.Path)
        {
            GridArray[P.x, P.y].GetComponent<EditorCell>().Clicked = true;
            GridArray[P.x, P.y].GetComponent<Image>().color = Color.gray;
        }

        //Change the current cell to red
        Vector2Int LastPos = preset.Path[preset.Path.Count - 1];
        GridArray[LastPos.x, LastPos.y].GetComponent<Image>().color = Color.red;
    }

    public void DestroyGrid()
    {
        foreach(Transform t in GridArray)
        {
            Destroy(t.gameObject);
        }
    }

    void InitializeValues()
    { 
        //Distance between cells
        offset = 2;

        //Get the Width/Height from the Background
        screenWidth = Background.rect.width;
        screenHeight = Background.rect.height - 150;

        //Get the Side by dividing screen width minus the offset between cells by gridSizeX
        Side = (screenWidth - (GridSizeX - 1) * offset) / GridSizeX;
        ScaleObjects();

        //Get the gridSizeY by getting the floor of the division of screenHeight by the Side + offset
        GridSizeY = Mathf.FloorToInt(screenHeight / (Side + offset));
    }

    void ScaleObjects()
    {
        editorCell.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(Side, Side);
        LevelEditorManager.Instance.Enemy.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(Side, Side);

        //Get the starting position
        startPosition = new Vector2(editorCell.GetComponent<Image>().rectTransform.rect.width / 2, editorCell.GetComponent<Image>().rectTransform.rect.height / 2);
    }

    #endregion
}
