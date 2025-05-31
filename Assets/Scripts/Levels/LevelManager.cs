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

        // 清除旧关卡数据
        levels.Clear();

        // 通知 RoomManager 加载房间数据
        GameManager.Instance.roomManager.LoadJson(levelName);

        // 获取 RoomManager 中的房间数据（已加载）
        var loadedRooms = GameManager.Instance.roomManager.roomDict;

        // 生成新 Level
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
