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

    //��ʼ����
    public void StartWave(Room room)
    {
        currentRoom = room;
        GameManager.Instance.currentWave = 1;

        CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
    }
    //��һ��
    private void NextWave(Room room)
    {
        GameManager.Instance.currentWave++;

        // ���������һ��
        if (GameManager.Instance.currentWave <= room.waves.Count)
        {
            CoroutineManager.Instance.StartManagedCoroutine("EnemySpawn", "wave " + GameManager.Instance.currentWave, SpawnWave(room));
        }
        else
        {
            // ���в������
            WaveEnd();
        }
    }
    //���в��ν���
    private void WaveEnd()
    {
        GameManager.Instance.state = GameManager.GameState.WAVEEND;

        // ��ǵ�ǰ�������
        if (currentRoom != null)
        {
            currentRoom.isCleared = true;
            currentRoom.isActive = false;
        }
            

        // ���Ż������������
        /*LevelController levelController = FindObjectOfType<LevelController>();
        if (levelController != null)
        {
            levelController.OpenRoomGates(currentRoom);
        }*/

        Debug.Log($"Room {currentRoom.name} cleared.");
    }
    //���˲����Э��
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

        // �ȴ����е�������
        yield return new WaitWhile(() => GameManager.Instance.enemyManager.enemy_count > 0);

        if (GameManager.Instance.state != GameManager.GameState.GAMEOVER)
        {
            NextWave(room);
        }
    }

    
    //spawn enemy ����
    private void SpawnEnemy(EnemyCharacter character, Vector3 pos)
    {
        GameObject enemyObj = Instantiate(enemy, pos, Quaternion.identity);
        
        enemyObj.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(character.enemySprite.spriteIndex);
        
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.Init(character);//��ʼ������ ����û�й��췽����ʹ��init��ʼ��
        controller.character.gameObject = enemyObj; //set gameObject
        controller.character.StartWave();
        GameManager.Instance.enemyManager.AddEnemy(enemyObj);
    }
    //��ȡ����λ��
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

        // ������Զ�κ���Ȼʧ�ܣ��ͷ������һ�ε�λ�ã���������ҽϽ���
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
