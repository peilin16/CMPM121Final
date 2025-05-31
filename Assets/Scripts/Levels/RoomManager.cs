using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public Transform player;

    public Dictionary<string, Room> roomDict = new Dictionary<string, Room>();
    public Room currentRoom;

    void Start()
    {
        // ��ʼ�����ӳ����������� room GameObject���� Room_1_1��
        foreach (var go in GameObject.FindGameObjectsWithTag("Room"))
        {
            BoxCollider2D collider = go.GetComponent<BoxCollider2D>();
            if (collider)
            {
                Bounds bounds = collider.bounds;
                string name = go.name;
                int id = name.GetHashCode(); // �� ID�����滻Ϊ�Զ���

                Room room = new Room(name, id, bounds);
                roomDict[name] = room;
            }
        }
    }

    void Update()
    {
        Vector3 pos = player.position;
        Room room = GetRoomFromPosition(pos);
        if (room != null && currentRoom != room)
        {
            currentRoom = room;
            currentRoom.isExplored = true;
            Debug.Log($"Entered Room: {currentRoom.name}");
        }
    }

    public Room GetRoomFromPosition(Vector3 pos)
    {
        foreach (var room in roomDict.Values)
        {
            if (room.Contains(pos))
                return room;
        }

        return null;
    }
}