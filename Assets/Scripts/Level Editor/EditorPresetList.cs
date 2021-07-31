using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorPresetList 
{
    public EditorPreset[] Levels;

    public EditorPresetList(int LevelCount)
    {
        Levels = new EditorPreset[LevelCount];
    }
}
