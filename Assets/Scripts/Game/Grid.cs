using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Grid : MonoBehaviour
{
    public static Grid Instance;
    public Transform Cell;
    public Transform[,] GridArray;
    public int GridSizeX, GridSizeY;
    public Transform StartCell;
    public Transform EndCell;


    //Initialize Singleton
    public void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    //Instantiate the grid
    public void DrawGrid(EditorPreset preset, float offset)
    {

        //Initialize the grid variable
        GridSizeX = preset.GridSizeX;
        GridSizeY = preset.GridSizeY;
        GridArray = new Transform[GridSizeX, GridSizeY];

        //Get the starting position
        Vector2 StartPosition = new Vector2(Cell.GetComponent<Image>().rectTransform.rect.width / 2 + offset, Cell.GetComponent<Image>().rectTransform.rect.height / 2 + offset * 0.5f);

        Vector2 CurrentPosition = StartPosition;
        Vector2Int currentCell = new Vector2Int();

        //Create the grid, loop through the row and columns start at the bottom left
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                //Instantiate the cell and place it correctly on the screen
                Transform g = Instantiate(Cell, this.transform);
                g.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(CurrentPosition.x, CurrentPosition.y);

                //Add an event listener on click to move the player to the cell
                g.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.MovePlayer(g));

                //Sets the cell position in the grid
                g.GetComponent<Cell>().SetXY(x, y , CurrentPosition);
                GridArray[x, y] = g;

                currentCell = new Vector2Int(x, y);

                //Move the current position by the side + spacing on the x axis
                CurrentPosition = new Vector2(CurrentPosition.x + preset.Side + 2, CurrentPosition.y);
            }

            //Move the current position by the side + spacing on the y axis and reset the x to the starting x
            CurrentPosition = new Vector2(StartPosition.x, CurrentPosition.y + preset.Side + 2);
        }

        //Get the neighbors of every cell and make them visible
        foreach(Transform t in GridArray)
        {
            t.GetComponent<Cell>().GetNeighbors();
            t.GetComponent<Image>().DOFade(1, 0.5f);
        }

        //Load the start cell from the preset
        StartCell = GridArray[preset.Path[0].x, preset.Path[0].y];
        EndCell = GridArray[preset.Path[preset.Path.Count-1].x, preset.Path[preset.Path.Count - 1].y];

        //Set start/end cell as inactive to disable going back
        StartCell.gameObject.SetActive(false);
        EndCell.gameObject.SetActive(false);

        //Hide enemies behind their cell
        foreach(Vector2Int V in preset.EnemyPositions)
        {
            Cell cell = GridArray[V.x, V.y].GetComponent<Cell>();
            cell.Enemy = true;
            EventManager.spawn(cell.Position);
        }
    }


    //Destroy the grid
    public void DestroyGrid()
    {
        for (int y = 0; y < GridSizeY; y++)
        {
            for (int x = 0; x < GridSizeX; x++)
            {
                Destroy(GridArray[x, y].gameObject);
            }
        }
    }
}
