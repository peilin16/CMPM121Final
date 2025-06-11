using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

public class EnemyManager
{
    public List<GameObject> enemies = new List<GameObject>();
    private long _id;
    public long Controller_ID
    {
        get => _id;
        private set => _id = GameManager.Instance.GenerateID();
    }
    public int enemy_count { get { return enemies.Count; } }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        GameObject.Destroy(enemy);
    }

    public void DestroyAllEnemies()
    {
        Debug.Log("Destory" + enemy_count);

        // 创建副本以避免修改原列表
        List<GameObject> toDestroy = new List<GameObject>(enemies);

        foreach (GameObject enemy in toDestroy)
        {
            if (enemy == null) continue;

            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Die(true);
            }
        }

        enemies.Clear(); // 最后清空一次
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a, b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }






}
