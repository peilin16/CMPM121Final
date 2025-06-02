// Room.cs
using UnityEngine;
using System.Collections.Generic;



public class Room
{
    public string name; //����
    public int roomID;//ID
    public Bounds bounds;//����
    public bool isActive = false;//�Ƿ񼤻�
    public bool isCleared = false;//�Ƿ��ѱ�̽��
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