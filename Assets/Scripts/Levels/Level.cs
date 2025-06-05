using UnityEngine;
using System.Collections.Generic;

public class Level
{
    public long level_id;
    public string level_name;
    public List<Room> rooms = new List<Room>();




    public Room GetRoomFromPosition(Vector2 position)
    {
        foreach (Room room in rooms)
        {
            if (room.bounds.Contains(position))
                return room;
        }
        return null;
    }

}
