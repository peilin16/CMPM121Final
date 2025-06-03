using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
public class RoomManager : MonoBehaviour
{
    public Transform player;

    public Dictionary<string, Room> roomDict = new Dictionary<string, Room>();
    public Room currentRoom;

    void Start()
    {
        // 初始化：从场景中找所有 room GameObject（如 Room_1_1）
        foreach (var go in GameObject.FindGameObjectsWithTag("Room"))
        {
            BoxCollider2D collider = go.GetComponent<BoxCollider2D>();
            if (collider)
            {
                Bounds bounds = collider.bounds;
                string name = go.name;
                int id = name.GetHashCode(); // 简化 ID，可替换为自定义

                Room room = new Room(name, id, bounds);
                roomDict[name] = room;
            }
        }
        GameManager.Instance.roomManager = this;
        //this.LoadJson("level1_rooms");
    }

    //主要它只会加载当前level的room如果需要加载其他level请先清空room的list
    public void LoadJson(string level)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(level);
        if (jsonText == null)
        {
            Debug.LogError("{level}.json not found in Resources folder.");
            return;
        }

        var enemyManager = GameManager.Instance.enemyCharacterManager;

        JArray root = JArray.Parse(jsonText.text);
        foreach (var roomObj in root)
        {
            string name = roomObj["name"].ToString();
            if (!roomDict.TryGetValue(name, out Room room))
            {
                Debug.LogWarning($"Room {name} not found in scene.");
                continue;
            }

            var wavesArray = roomObj["waves"] as JArray;
            foreach (var waveObj in wavesArray)
            {
                List<EnemyCharacter> waveList = new List<EnemyCharacter>();

                foreach (var enemyObj in waveObj["enemies"])
                {
                    string type = enemyObj["enemy"]?.ToString();
                    int count = (int)(enemyObj["count"] ?? 1);

                    EnemyCharacter baseCharacter = enemyManager.GetEnemy(type);
                    if (baseCharacter == null)
                    {
                        Debug.LogWarning($"Enemy type {type} not found in EnemyCharacterManager.");
                        continue;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        EnemyCharacter clone = baseCharacter.Clone();

                        if (enemyObj["hp"] != null)
                            clone.final_healthly = (int)enemyObj["hp"];
                        else
                            clone.final_healthly = baseCharacter.final_healthly;

                        if (enemyObj["damage"] != null)
                            clone.final_damage = (float)enemyObj["damage"];
                        else
                            clone.final_damage = baseCharacter.final_damage;

                        if (enemyObj["speed"] != null)
                            clone.final_speed = (float)enemyObj["speed"];
                        else
                            clone.final_speed = baseCharacter.final_speed;

                        waveList.Add(clone);
                    }
                }

                room.waves.Add(waveList);
            }

            Debug.Log($"Room {name} loaded with {room.waves.Count} waves.");
        }
    }



    void Update()
    {
        /*Vector3 pos = player.position;
        Room room = GetRoomFromPosition(pos);
        if (room != null && currentRoom != room)
        {
            currentRoom = room;
            //currentRoom.isCleared = true;
            Debug.Log($"Entered Room: {currentRoom.name}");
        }*/
    }
    //不推荐使用 请改用level class内部的GetRoomFromPosition
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