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
       set => enemy = (EnemyCharacter)value;
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
        Controller_ID = GameManager.Instance.GenerateID();
        target = GameManager.Instance.player.transform;
        //hp.OnDeath += Die;
        EventBus.Instance.OnMonsterDeath += (gameObject) => this.Die();

        /*Collider2D enemyCol = GetComponent<Collider2D>();
        Collider2D playerCol = GameManager.Instance.player.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(enemyCol, playerCol);*/
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }    // Update is called once per frame
    void Update()
    {
        enemy.Behavior(gameObject); //  character
        
        // Add spell casting behavior
        if (enemy != null && GameManager.Instance.player != null)
        {
            enemy.UpdateBehavior(GameManager.Instance.player.transform.position);
        }
        
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
    public void Die(bool isDestory = false)
    {
        
        if (isDestory == true || (!dead && this.enemy.hp.hp <= 0))
        {
            EventBus.Instance.OnMonsterDamaged -= this.BeHitting;
            EventBus.Instance.OnMonsterDeath -= (gameObject) => this.Die();
            //enemy.gameObject = null;
            //this.enemy = null;
            dead = true;
            Debug.Log("Die");
            //Destroy(this.gameObject);
            Destroy(this.enemy.gameObject); // Redundant & dangerous
            GameManager.Instance.enemyManager.RemoveEnemy(this.gameObject);
            
        }
    }
}
