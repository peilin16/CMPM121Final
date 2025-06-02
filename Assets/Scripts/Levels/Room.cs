// Room.cs
using UnityEngine;
using System.Collections.Generic;



public class Room
{
    public string name; //名字
    public int roomID;//ID
    public Bounds bounds;//对象
    public bool isActive = false;//是否激活
    public bool isCleared = false;//是否已被探索
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