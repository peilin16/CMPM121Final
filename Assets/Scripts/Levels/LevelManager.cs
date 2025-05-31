using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LevelManager
{
    public Dictionary<string, LevelData> levels = new Dictionary<string, LevelData>();

    public LevelManager()
    {
        LoadLevels(1);
    }
    /*
    public LevelData GetLevel(string name)
    {
        if (levels.TryGetValue(name, out LevelData level))
            return level;

        Debug.LogWarning($"Level '{name}' not found.");
        return null;
    }*/

    public void LoadLevels(int level)
    {
        string levelName = $"level{level}";

        // ����ɹؿ�����
        levels.Clear();

        // ֪ͨ RoomManager ���ط�������
        GameManager.Instance.roomManager.LoadJson(levelName);

        // ��ȡ RoomManager �еķ������ݣ��Ѽ��أ�
        var loadedRooms = GameManager.Instance.roomManager.roomDict;

        // ������ Level
        Level newLevel = new Level
        {
            level_id = level,
            level_name = levelName,
            rooms = new List<Room>(loadedRooms.Values)
        };

        levels[levelName] = newLevel;

        Debug.Log($"Level {levelName} loaded with {newLevel.rooms.Count} rooms.");
    }







}
