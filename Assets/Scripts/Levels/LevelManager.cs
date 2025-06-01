using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class LevelManager
{
    public Dictionary<string, Level> levels = new Dictionary<string, Level>();

    public LevelManager()
    {
        //LoadLevels(GameManager.Instance.currentLevel);
    }
    
    public Level GetLevel(string name)
    {
        if (levels.TryGetValue(name, out Level level))
            return level;

        Debug.LogWarning($"Level '{name}' not found.");
        return null;
    }

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
