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

    //开始波次
    public void StartWave(Room room)
    {
        currentRoom = room;
        GameManager.Instance.currentWave = 1;

        CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
    }
    //下一波
    private void NextWave(Room room)
    {
        GameManager.Instance.currentWave++;

        // 如果还有下一波
        if (GameManager.Instance.currentWave <= room.waves.Count)
        {
            CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
        }
        else
        {
            // 所有波次完成
            WaveEnd();
        }
    }
    //所有波次结束
    private void WaveEnd()
    {
        GameManager.Instance.state = GameManager.GameState.WAVEEND;

        // 标记当前房间完成
        if (currentRoom != null)
        {
            currentRoom.isCleared = true;
            currentRoom.isActive = false;
        }
            

        // 打开门或进行其他处理
        /*LevelController levelController = FindObjectOfType<LevelController>();
        if (levelController != null)
        {
            levelController.OpenRoomGates(currentRoom);
        }*/

        Debug.Log($"Room {currentRoom.name} cleared.");
    }
    //敌人部署的协程
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
            Vector3 spawnPos = GetRandomPos(room);
            this.SpawnEnemy(enemyChar, spawnPos);
        }

        // 等待所有敌人死亡
        yield return new WaitWhile(() => GameManager.Instance.enemyManager.enemy_count > 0);

        if (GameManager.Instance.state != GameManager.GameState.GAMEOVER)
        {
            NextWave(room);
        }
    }

    
    //spawn enemy 方法
    private void SpawnEnemy(EnemyCharacter character, Vector3 pos)
    {
        GameObject enemyObj = Instantiate(enemy, pos, Quaternion.identity);
        
        enemyObj.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(character.enemySprite.spriteIndex);
        
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.Init(character);//初始化敌人 由于没有构造方法请使用init初始化
        controller.character.gameObject = enemyObj; //set gameObject
        controller.character.StartWave();
        GameManager.Instance.enemyManager.AddEnemy(enemyObj);
    }
    //获取部署位置
    private Vector3 GetRandomPos(Room room)
    {
        int maxTries = 20;
        float minDistanceToPlayer = 2.0f;
        Vector3 playerPos = GameManager.Instance.player.transform.position;

        for (int i = 0; i < maxTries; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(room.bounds.min.x + 0.5f, room.bounds.max.x - 0.5f),
                Random.Range(room.bounds.min.y + 0.5f, room.bounds.max.y - 0.5f),
                0
            );

            if (Vector3.Distance(spawnPos, playerPos) >= minDistanceToPlayer)
            {
                return spawnPos;
            }
        }

        // 如果尝试多次后仍然失败，就返回最后一次的位置（可能离玩家较近）
        return new Vector3(
            Random.Range(room.bounds.min.x + 0.5f, room.bounds.max.x - 0.5f),
            Random.Range(room.bounds.min.y + 0.5f, room.bounds.max.y - 0.5f),
            0
        );
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
