using UnityEngine;
using System.Collections.Generic;

public class TreasureManager { 

    private Dictionary<string, Treasure> treasures = new Dictionary<string, Treasure>();



    public void RegisterTreasure(Treasure treasure)
    {
        if (!treasures.ContainsKey(treasure.treasureID))
        {
            treasures[treasure.treasureID] = treasure;
            Debug.Log($"Registered {treasure.treasureID} at {treasure.GetPosition()}");
        }
    }

    public void OpenTreasure(string id)
    {
        if (treasures.TryGetValue(id, out var treasure))
        {
            Debug.Log($"Treasure {id} marked as opened");
            // 这里你可以记录进档案、统计进度等
        }
    }

    public Dictionary<string, Treasure> GetAllTreasures()
    {
        return treasures;
    }
}
