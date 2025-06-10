using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    //public Image level_selector;

    private Room currentRoom;
    private int currentWave;
    public GameObject enemy;

    [SerializeField] private Tilemap wallTilemap; // 

    void Start()
    {

    }


    public void StartWave(Room room)
    {
        currentRoom = room;
        GameManager.Instance.currentWave = 1;

        CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
    }

    private void NextWave(Room room)
    {
        GameManager.Instance.currentWave++;

        // if exit next wave
        if (GameManager.Instance.currentWave <= room.waves.Count)
        {
            CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
        }
        else
        {
            // 
            WaveEnd();
        }
    }
    //end
    private void WaveEnd()
    {
        GameManager.Instance.state = GameManager.GameState.WAVEEND;

        // 
        if (currentRoom != null)
        {
            currentRoom.isCleared = true;
            currentRoom.isActive = false;
        }

        EventBus.Instance.TriggerCleanRoomAction(GameManager.Instance.playerController);


        EventCenter.Broadcast(EventDefine.ShowSpellSelectorPanel);
        Debug.Log($"Room {currentRoom.name} cleared.");
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
            Vector3 spawnPos = GetRandomPos(room);
            this.SpawnEnemy(enemyChar, spawnPos);
        }


        yield return new WaitWhile(() => GameManager.Instance.enemyManager.enemy_count > 0);

        if (GameManager.Instance.state != GameManager.GameState.GAMEOVER)
        {
            NextWave(room);
        }
    }

    
    //spawn enemy 
    private void SpawnEnemy(EnemyCharacter character, Vector3 pos)
    {
        GameObject enemyObj = Instantiate(enemy, pos, Quaternion.identity);
        
        enemyObj.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(character.enemySprite.spriteIndex);
        
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.Init(character);
        controller.character.gameObject = enemyObj; //set gameObject
        GameManager.Instance.enemyManager.AddEnemy(enemyObj);
    }







    //get spawn position

    private Vector3 GetRandomPos(Room room,float minWallDistance = 0.6f)
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

            // if too close with player
            if (Vector3.Distance(spawnPos, playerPos) < minDistanceToPlayer)
                continue;

            // if too close with wall
            if (IsNearWall(spawnPos, minWallDistance))
                continue;

            return spawnPos;
        }

        // fallback 
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = new Vector3(
                Random.Range(room.bounds.min.x + 0.5f, room.bounds.max.x - 0.5f),
                Random.Range(room.bounds.min.y + 0.5f, room.bounds.max.y - 0.5f),
                0
            );
            if (!IsNearWall(spawnPos, minWallDistance))
                return spawnPos;
        }

        return room.bounds.center;
    }

    private bool IsNearWall(Vector3 position, float minWallDistance)
    {
        Vector3Int centerCell = wallTilemap.WorldToCell(position);
        int range = Mathf.CeilToInt(minWallDistance / wallTilemap.cellSize.x); // 假设方格宽高一致

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                Vector3Int checkCell = centerCell + new Vector3Int(dx, dy, 0);
                if (wallTilemap.HasTile(checkCell))
                    return true;
            }
        }

        return false;
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
