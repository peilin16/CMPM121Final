using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,//游戏开始的状态
        INWAVE, //玩家在房间内受到攻击的状态
        WAVEEND,//final 不再使用 waveEnd 状态

        EXPEDITION,//当玩家自由探索地图时的状态
        COUNTDOWN,
        GAMEOVER//游戏结束

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
    //自增式ID生成
    public long GenerateID()
    {
        return _nextID++;
    }

    //Level 相关
    public LevelManager levelManager;
    public RoomManager roomManager;
    //spell 相关
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public SpellBuilder spellBuilder;
    //enemy相关
    public EnemySpriteManager enemySpriteManager;
    public EnemyCharacterManager enemyCharacterManager;
    public EnemyManager enemyManager;
    //player相关
    public GameObject player;
    public PlayerSpriteManager playerSpriteManager;
    //relic相关
    public RelicIconManager relicIconManager;
    public RelicManager relicManager;
    //reward 相关
    //public RewardManager rewardManager;
    //其他
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
