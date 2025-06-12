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
            // ��������Լ�¼��������ͳ�ƽ��ȵ�
        }
    }

    public Dictionary<string, Treasure> GetAllTreasures()
    {
        return treasures;
    }
}
