using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LevelManager
{
    public Dictionary<string, Level> levels = new Dictionary<string, Level>();
    public int currentLevel = 1;
    public string currentLevelName;
    public LevelManager()
    {
        //LoadLevels(GameManager.Instance.currentLevel);
    }
    
    public void setLevel(int level)
    {

    }

    public Level GetLevel()
    {
        string levelName = $"level{currentLevel}";

        if (levels.Count == 0)
            LoadLevels(currentLevel);


        if (levels.TryGetValue(levelName, out Level level))
            return level;

        Debug.LogWarning($"Level '{levelName}' not found.");
        return null;
    }
    public void LoadLevels(int level)
    {
        string levelName = $"level{level}";
        currentLevelName = levelName;
        // 清除旧关卡数据
        levels.Clear();
        string JsonName = $"level{level}_rooms";
        // 通知 RoomManager 加载房间数据
        GameManager.Instance.roomManager.LoadJson(JsonName);

        // 获取 RoomManager 中的房间数据（已加载）
        var loadedRooms = GameManager.Instance.roomManager.roomDict;

        // 生成新 Level
        Level newLevel = new Level
        {
            level_id = GameManager.Instance.GenerateID(),
            level_name = levelName,
            rooms = new List<Room>(loadedRooms.Values)
        };

        levels[levelName] = newLevel;

        Debug.Log($"Level {levelName} loaded with {newLevel.rooms.Count} rooms.");
    }







}
