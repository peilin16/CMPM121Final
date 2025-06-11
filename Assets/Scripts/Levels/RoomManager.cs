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
        // init
        foreach (var go in GameObject.FindGameObjectsWithTag("Room"))
        {
            BoxCollider2D collider = go.GetComponent<BoxCollider2D>();
            if (collider)
            {
                Bounds bounds = collider.bounds;
                string name = go.name;
                int id = name.GetHashCode(); 

                Room room = new Room(name, id, bounds);
                roomDict[name] = room;
            }
        }
        GameManager.Instance.roomManager = this;
        //this.LoadJson("level1_rooms");
    }

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
        //LoadVictoryRoom();
    }
    public void LoadVictoryRoom()
    {
        GameObject victoryGO = GameObject.Find("Victory_Room");
        if (victoryGO == null)
        {
            Debug.LogWarning("Victory_Room not found in scene.");
            return;
        }

        BoxCollider2D collider = victoryGO.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogWarning("Victory_Room has no BoxCollider2D.");
            return;
        }

        Bounds bounds = collider.bounds;
        string name = victoryGO.name;
        int id = name.GetHashCode();

        Room room = new Room(name, id, bounds);
        room.isCleared = true;   //
        room.isActive = false;

        roomDict[name] = room;

        Debug.Log(room.name+" loaded into roomDict.");
    }





    //Not Recommand
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