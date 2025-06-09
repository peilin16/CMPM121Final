using UnityEngine;
using UnityEngine.Tilemaps;


[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour, Controller
{

    public Transform target;

    public HealthBar healthui;
    public bool dead;
    //public Hittable hp;
    public float speed;

    public EnemyCharacter enemy;  // Implements Controller.character
    public float last_attack;






    public Character character
    {
        get => enemy;
       set => enemy = (EnemyCharacter)value;//有遗留问题 直接调用enemy对象 不要用set不然会覆盖掉原有的设置
    }

    public HealthBar HealthUI
    {
        get => healthui;
        set => healthui = value;
    }

    public bool IsDead
    {
        get => dead;
        set => dead = value;
    }
    private long _id;
    public long Controller_ID
    {
        get => _id;
        private set => _id = value;
    }


    //movement logical
    protected EnemyMovement movement;
    private Transform playerTransform;
    private Tilemap wallTilemap;
    



    //public EnemyCharacter enemy;
    // init controller
    public void Init(EnemyCharacter character)
    {
        this.enemy = character;
        enemy.gameObject = this.gameObject;
        this.enemy.InitController(this);
        healthui.SetHealth(enemy.hp);

        EventBus.Instance.OnMonsterDamaged += this.BeHitting;
        playerTransform = GameManager.Instance.player.transform;
        wallTilemap = GameObject.Find("Wall").GetComponent<Tilemap>();
        //init movement
        movement = new EnemyMovement(this, wallTilemap, LayerMask.GetMask("Wall"));


        /*Collider2D enemyCol = GetComponent<Collider2D>();
        Collider2D playerCol = GameManager.Instance.player.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(enemyCol, playerCol);*/
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controller_ID = GameManager.Instance.GenerateID();
        target = GameManager.Instance.player.transform;
        //hp.OnDeath += Die;
        EventBus.Instance.OnMonsterDeath += (gameObject) => this.Die();
        
    }

    // Update is called once per frame
    void Update()
    {
        enemy.Behavior(gameObject); //  character
        
            //Debug.Log("moving");
        movement.MoveTowards(enemy.destination, enemy.stopDistance);//enemy  moving

    }

    /*
    public EnemyCharacter GetCharacter()
    {
        return character;
    }*/



    void StartLevel()
    {
        enemy.StartLevel();
    }

    void DoAttack()
    {
        if (last_attack + 2 < Time.time)
        {
            last_attack = Time.time;
            target.gameObject.GetComponent<PlayerController>().player.hp.Damage(new Damage(5, Damage.Type.PHYSICAL));
        }
    }

    public void BeHitting(Damage damage, GameObject obj)
    {
        if (obj == this.gameObject)
        {
            //Debug.Log("bbb");
            this.healthui.SetHealth(enemy.hp);
        }
        
    }
    public void Die()
    {
        if (!dead && this.enemy.hp.hp <= 0)
        {
            EventBus.Instance.OnMonsterDamaged -= this.BeHitting;
            EventBus.Instance.OnMonsterDeath -= (gameObject) => this.Die();
            dead = true;
            GameManager.Instance.enemyManager.RemoveEnemy(this.gameObject);
            Destroy(this.gameObject);
            Destroy(this.enemy.gameObject);
        }
    }
}
