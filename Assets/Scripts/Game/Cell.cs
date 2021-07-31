using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool Enemy; //If true an Enemy is hidden behind this cell
    public List<Cell> Neighbors;
    public int GridX;
    public int GridY;
    public Vector2 Position;


    //Set the Position in the Grid
    public void SetXY(int x , int y , Vector2 Pos)
    {
        GridX = x;
        GridY = y;
        Position = Pos;
    }



    //Get the neighbors of the current cell in the grid
    public void GetNeighbors()
    {
        Neighbors = new List<Cell>();

        //Get the left neighbor
        if (GridX > 0)
        {
            Neighbors.Add(Grid.Instance.GridArray[GridX - 1, GridY].GetComponent<Cell>());
        }

        //Get the right neighbor
        if (GridX < Grid.Instance.GridSizeX - 1)
        {
            Neighbors.Add(Grid.Instance.GridArray[GridX + 1, GridY].GetComponent<Cell>());
        }

        //Get the bottom neighbor
        if (GridY > 0)
        {
            Neighbors.Add(Grid.Instance.GridArray[GridX, GridY - 1].GetComponent<Cell>());
        }

        //Get the top neighbor
        if (GridY < Grid.Instance.GridSizeY - 1)
        {
            Neighbors.Add(Grid.Instance.GridArray[GridX, GridY + 1].GetComponent<Cell>());
        }
    }



    //Check if the player can move to a neighbor cell or no
    public bool Check()
    {
        bool Status = false;
        
        //if all neighbors are deactivated the game ends
        foreach(Cell C in Neighbors)
        {
            if(C.gameObject.activeInHierarchy)
            {
                Status = true;
            }
        }

        return Status;
    }
}
