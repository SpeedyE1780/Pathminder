using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorEnemy : MonoBehaviour
{
    public EditorCell currentCell;

    void SetCell(EditorCell C)
    {
        currentCell = C;
    }
}
