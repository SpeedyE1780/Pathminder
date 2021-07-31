using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCell : MonoBehaviour
{
    public List<EditorCell> Neighbors;
    public Vector2Int GridPos;
    public Vector2 Postion;
    public bool Clicked;

    //Set the Position in the Grid
    public void SetXY(int x , int y , Vector2 Pos)
    {
        GridPos = new Vector2Int(x, y);
        Postion = Pos;
    }

    //Get the neighbors of the current cell in the grid
    public void GetNeighbors()
    {
        Neighbors = new List<EditorCell>();

        //Get the left neighbor
        if (GridPos.x > 0)
        {
            Neighbors.Add(GridManager.Instance.GridArray[GridPos.x - 1, GridPos.y].GetComponent<EditorCell>());
        }

        //Get the right neighbor
        if (GridPos.x < GridManager.Instance.GridSizeX - 1)
        {
            Neighbors.Add(GridManager.Instance.GridArray[GridPos.x + 1, GridPos.y].GetComponent<EditorCell>());
        }

        //Get the bottom neighbor
        if (GridPos.y > 0)
        {
            Neighbors.Add(GridManager.Instance.GridArray[GridPos.x, GridPos.y - 1].GetComponent<EditorCell>());
        }

        //Get the top neighbor
        if (GridPos.y < GridManager.Instance.GridSizeY - 1)
        {
            Neighbors.Add(GridManager.Instance.GridArray[GridPos.x, GridPos.y + 1].GetComponent<EditorCell>());
        }
    }
}
