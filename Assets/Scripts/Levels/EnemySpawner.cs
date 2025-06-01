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
    


    void Start()
    {
        /*CreateButton("Easy", 40);
        CreateButton("Medium", 0);
        CreateButton("Endless", -40);*/
    }

    void CreateButton(string label, float yOffset)
    {
       // GameObject btn = Instantiate(button, level_selector.transform);
        /*btn.transform.localPosition = new Vector3(0, yOffset);
        btn.GetComponent<MenuSelectorController>().spawner = this;
        btn.GetComponent<MenuSelectorController>().SetLevel(label);*/
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
            GameManager.Instance.enemyManager.SpawnEnemy(enemyChar, spawnPos); // 你已有的函数
        }

        // 等待所有敌人死亡
        yield return new WaitWhile(() => GameManager.Instance.enemyManager.enemy_count > 0);

        if (GameManager.Instance.state != GameManager.GameState.GAMEOVER)
            GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    /*
    //spawn enemy
    void SpawnEnemy(EnemyCharacter character, string location)
    {
        Vector3 pos = PickSpawnPoint(location);
        GameObject enemyObj = Instantiate(enemy, pos, Quaternion.identity);
        
        enemyObj.GetComponent<SpriteRenderer>().sprite =
            GameManager.Instance.enemySpriteManager.Get(character.enemySprite.spriteIndex);
        
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.character = character; // assign the character first
        controller.character.gameObject = enemyObj; //set gameObject
        controller.character.StartWave();
        GameManager.Instance.enemyManager.AddEnemy(enemyObj);
    }

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
