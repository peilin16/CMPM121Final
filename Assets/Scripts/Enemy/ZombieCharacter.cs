// ZombieCharacter.cs
using UnityEngine;

public class ZombieCharacter : EnemyCharacter
{
    public ZombieCharacter(EnemySprite sprite, string typ) : base(sprite, typ) {

    }




    public override EnemyCharacter Clone()
    {
        return new ZombieCharacter(this.enemySprite, this.type)
        {
            final_healthly = this.final_healthly,
            final_damage = this.final_damage,
            final_speed = this.final_speed

        };
    }
    public override void Behavior(GameObject gameObject)
    {
        //if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        //Debug.Log("aaa");
        Vector3 direction = GameManager.Instance.player.transform.position - gameObject.transform.position;
        float distance = direction.magnitude;

        if (distance <= 1.1f)
        {
            //Debug.Log("aaa");
            isMove = false;
            if (controller.last_attack + 2f < Time.time)
            {
                controller.last_attack = Time.time;
                GameManager.Instance.playerController.player.hp.Damage(new Damage(5, Damage.Type.PHYSICAL));
            }
        }
        else
        {
            this.destination = GameManager.Instance.player.transform.position;
            isMove = true;
        }
    }
}