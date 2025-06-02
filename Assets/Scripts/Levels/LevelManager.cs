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
        // ����ɹؿ�����
        levels.Clear();
        string JsonName = $"level{level}_rooms";
        // ֪ͨ RoomManager ���ط�������
        GameManager.Instance.roomManager.LoadJson(JsonName);

        // ��ȡ RoomManager �еķ������ݣ��Ѽ��أ�
        var loadedRooms = GameManager.Instance.roomManager.roomDict;

        // ������ Level
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
