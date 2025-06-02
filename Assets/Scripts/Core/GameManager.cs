using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,//��Ϸ��ʼ��״̬
        INWAVE, //����ڷ������ܵ�������״̬
        WAVEEND,//final ����ʹ�� waveEnd ״̬

        EXPEDITION,//���������̽����ͼʱ��״̬
        COUNTDOWN,
        GAMEOVER//��Ϸ����

    }
    public GameState state;
    public enum Difficultly {Easy, Medium, Endless}
    public Difficultly level = Difficultly.Easy;
    public int currentWave = 1;
    public int maxWaves = 0;
    public int countdown;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
            {
                theInstance = new GameManager();
                theInstance.relicManager = new RelicManager();
                theInstance.enemyCharacterManager = new EnemyCharacterManager();

                theInstance.spellBuilder = new SpellBuilder();
                theInstance.enemyManager = new EnemyManager();
                //theInstance.roomManager = new RoomManager();
                theInstance.levelManager = new LevelManager();
            }    
            return theInstance;
        }
    }
    private static long _nextID = 0;
    //����ʽID����
    public long GenerateID()
    {
        return _nextID++;
    }

    //Level ���
    public LevelManager levelManager;
    public RoomManager roomManager;
    //spell ���
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public SpellBuilder spellBuilder;
    //enemy���
    public EnemySpriteManager enemySpriteManager;
    public EnemyCharacterManager enemyCharacterManager;
    public EnemyManager enemyManager;
    //player���
    public GameObject player;
    public PlayerSpriteManager playerSpriteManager;
    //relic���
    public RelicIconManager relicIconManager;
    public RelicManager relicManager;
    //reward ���
    //public RewardManager rewardManager;
    //����
    public int defectCount;
    public float waveSpendTime = 0f;
    public bool isTiming = false;
    public string difficultly = "Easy";

    public void RestartGame()
    {
        enemyManager.DestroyAllEnemies();
        CoroutineManager.Instance.StopGroup("EnemySpawn");
        //player.player.hp.hp = 1;
    }

    public void StartGame()
    {
        GameManager.Instance.state = GameManager.GameState.EXPEDITION;
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
    }

}
