using UnityEngine;
using Newtonsoft.Json.Linq;





public class EnemyCharacter : Character
{
    public EnemySprite enemySprite;
    private int _final_healthly;
    public int final_healthly
    {
        get => this._final_healthly;
        set
        {
            this._final_healthly = value;  
            if (this.hp == null && this.gameObject != null)
                this.hp = new Hittable(this._final_healthly, Hittable.Team.MONSTERS, gameObject);
            else if(this.hp != null)
                this.hp.SetMaxHP(_final_healthly);
        }
    }
    public float final_speed;
    public float final_damage;
    public Vector3 destination;
    //引入控制器对象
    protected EnemyController controller;
    public string type;

    public EnemyCharacter() { }

    public EnemyCharacter(EnemySprite sprite, string type)
    {
        this.enemySprite = sprite;
        this.type = type;
        this._final_healthly = sprite.healthly;
        this.final_speed = sprite.speed;
        this.final_damage = sprite.damage;
        
    }
    
    public override void StartLevel() {
        
    }
    public void InitController(EnemyController c)
    {
        //if (this.hp == null)
        this.hp = new Hittable(this._final_healthly, Hittable.Team.MONSTERS, gameObject);
        this.controller = c;
    }


    //当前默认向玩家移动的逻辑
    public virtual void Behavior(GameObject gameObject)
    {
        /*if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        Vector3 direction = GameManager.Instance.player.transform.position - gameObject.transform.position;
        float distance = direction.magnitude;

        if (distance < 2f)
        {
            // 简单近战攻击逻辑
            EnemyController ctrl = gameObject.GetComponent<EnemyController>();
            if (ctrl.last_attack + 2f < Time.time)
            {
                ctrl.last_attack = Time.time;
                GameManager.Instance.playerController.player.hp.Damage(new Damage(5, Damage.Type.PHYSICAL));
            }
        }
        else
        {
            gameObject.GetComponent<Unit>().movement = direction.normalized * final_speed;
        }*/
        //请根据自己的需求更改behavoir方法
    }

    protected override void JsonLoad(JObject obj)
    {
        //not use
    }


}
