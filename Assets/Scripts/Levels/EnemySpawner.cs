using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    //public Image level_selector;

    private Room currentRoom;
    private int currentWave;
    public GameObject enemy;


    void Start()
    {

    }


    public void StartWave(Room room)
    {
        currentRoom = room;
        GameManager.Instance.currentWave = 1;

        CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
    }

    public void NextWave()
    {
        /*if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            GameManager.Instance.currentWave += 1;
            currentLevel.NextWave();

            if (GameManager.Instance.currentWave <= currentLevel.total_waves)
                CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave());
            //StartCoroutine(SpawnWave());
            else
                Debug.Log("All waves complete.");
        }*/
    }
    public void WaveEnd()
    {

    }
    IEnumerator SpawnWave(Room room)
    {
        GameManager.Instance.state = GameManager.GameState.INWAVE;

        int waveIndex = GameManager.Instance.currentWave - 1;

        if (waveIndex < 0 || waveIndex >= room.waves.Count)
        {
            Debug.LogWarning("Wave index out of range.");
            yield break;
        }

        List<EnemyCharacter> waveEnemies = room.waves[waveIndex];

        foreach (var enemyChar in waveEnemies)
        {
            // 在房间边界内随机生成位置
            Vector3 spawnPos = new Vector3(
                Random.Range(room.bounds.min.x + 0.5f, room.bounds.max.x - 0.5f),
                Random.Range(room.bounds.min.y + 0.5f, room.bounds.max.y - 0.5f),
                0
            );

            enemyChar.StartWave(); // 初始化血量等
            this.SpawnEnemy(enemyChar, spawnPos); 
        }

        // 等待所有敌人死亡
        yield return new WaitWhile(() => GameManager.Instance.enemyManager.enemy_count > 0);

        /*if (GameManager.Instance.state != GameManager.GameState.GAMEOVER)
            GameManager.Instance.state = GameManager.GameState.WAVEEND;*/
    }

    
    //spawn enemy
    void SpawnEnemy(EnemyCharacter character, Vector3 pos)
    {
        GameObject enemyObj = Instantiate(enemy, pos, Quaternion.identity);
        
        enemyObj.GetComponent<SpriteRenderer>().sprite =
            GameManager.Instance.enemySpriteManager.Get(character.enemySprite.spriteIndex);
        
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.Init(character);//初始化敌人 由于没有构造方法请使用init初始化
        controller.character.gameObject = enemyObj; //set gameObject
        controller.character.StartWave();
        GameManager.Instance.enemyManager.AddEnemy(enemyObj);
    }
    /*
    Vector3 PickSpawnPoint(string location)
    {
        
        List<SpawnPoint> filtered = SpawnPoints
            .Where(p => location == "random" || p.tag.ToLower().Contains(location.Replace("random", "").Trim().ToLower()))
            .ToList();

        if (filtered.Count == 0) filtered = SpawnPoints.ToList();

        Vector3 basePos = filtered[UnityEngine.Random.Range(0, filtered.Count)].transform.position;
        Vector2 offset = UnityEngine.Random.insideUnitCircle * 1.5f;
        return basePos + new Vector3(offset.x, offset.y, 0);
    }*/
}
