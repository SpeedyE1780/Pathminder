using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void Respawn();
    public static Respawn respawn;

    public delegate void NextLevel();
    public static NextLevel nextLevel;

    public delegate void Spawn(Vector2 pos);
    public static Spawn spawn;

    public delegate void EnableControls();
    public static EnableControls enableControls;
}
