// Room.cs
using UnityEngine;
using System.Collections.Generic;



public class Room
{
    public string name;
    public int roomID;
    public Bounds bounds;
    public bool isActive = false;
    public bool isExplored = false;
    public List<List<EnemyCharacter>> waves = new List<List<EnemyCharacter>>();





    public Room(string name, int id, Bounds bounds)
    {
        this.name = name;
        this.roomID = id;
        this.bounds = bounds;
    }

    public bool Contains(Vector3 worldPos)
    {
        return bounds.Contains(worldPos);
    }
}