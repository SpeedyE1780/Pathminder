using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorPreset
{
    public int Level;
    public List<Vector2Int> Path;
    public List<Vector2Int> EnemyPositions;
    public int GridSizeX;
    public int GridSizeY;
    public float Side;

    public EditorPreset(int L, List<Vector2Int> P, List<Vector2Int> E, int gX, int gY, float S)
    {
        Level = L;

        Path = P;
        EnemyPositions = E;

        GridSizeX = gX;
        GridSizeY = gY;

        Side = S;
    }
}
