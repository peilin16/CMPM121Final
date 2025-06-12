using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE, 
        WAVEEND,//
        VICTORY,
        EXPEDITION,
        COUNTDOWN,
        GAMEOVER

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
                theInstance.recordCenter = new RecordCenter();
                theInstance.treasureManager = new TreasureManager();
            }    
            return theInstance;
        }
    }
    private static long _nextID = 0;
    //ID generate
    public long GenerateID()
    {
        return _nextID++;
    }

    //Level 
    public LevelManager levelManager;
    public RoomManager roomManager;
    //spell 
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public SpellBuilder spellBuilder;
    //enemy
    public EnemySpriteManager enemySpriteManager;
    public EnemyCharacterManager enemyCharacterManager;
    public EnemyManager enemyManager;
    //player
    public GameObject player;
    public PlayerController playerController;
    public PlayerSpriteManager playerSpriteManager;
    public Vector3 playerStartPosition;
    //relic
    public RelicIconManager relicIconManager;
    public RelicManager relicManager;
    //record 
    public RecordCenter recordCenter;
    //Other
    public UIManager uiManager;
    public TreasureManager treasureManager;

    //other
    public int defectCount;
    public float waveSpendTime = 0f;
    public bool isTiming = false;
    public string difficultly = "Easy";
    public GameObject StartPanel;
    public void RestartGame()
    {
        GameManager.Instance.state = GameManager.GameState.PREGAME;
        
        CoroutineManager.Instance.StopGroup("EnemySpawn");
        StartPanel.SetActive(true);

        player.transform.position = playerStartPosition;
        
        levelManager.InitLevel(levelManager.currentLevel);
        recordCenter.Init();
        playerController.ControllerInit();
        
        // Restart current scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //player.player.hp.hp = 1;
    }

    public void StartGame()
    {
        GameManager.Instance.state = GameManager.GameState.EXPEDITION;
        GameManager.Instance.playerController.StartLevel();
        
    }

}
