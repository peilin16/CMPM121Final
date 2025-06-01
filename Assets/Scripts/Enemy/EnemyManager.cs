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
        GameManager.Instance.defectCount++;
        enemies.Remove(enemy);
    }
    //�������еĵ��˶���
    public void DestroyAllEnemies()
    {
        List<GameObject> enemiesToDestroy = new List<GameObject>(enemies);
        foreach (GameObject enemy in enemiesToDestroy)
        {
            if (enemy != null)
            {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.Die();
                    GameObject.Destroy(enemy);
                }
            }
        }
        enemies.Clear();
    }
    //��ȡ����ĵ���
    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a, b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }


    //���˲��𷽷�
    public void SpawnEnemy(EnemyCharacter character, Vector3 pos)
    {

        GameObject enemyObj = Instantiate(enemy, pos, Quaternion.identity);
        enemyObj.GetComponent<SpriteRenderer>().sprite =
            GameManager.Instance.enemySpriteManager.Get(character.enemySprite.spriteIndex);
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.character = character; // assign the character first
        controller.character.gameObject = enemyObj; //set gameObject
        controller.character.StartWave();
        GameManager.Instance.enemyManager.AddEnemy(enemyObj);
    }



}
