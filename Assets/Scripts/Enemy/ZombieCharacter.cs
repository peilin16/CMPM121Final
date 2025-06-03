// ZombieCharacter.cs
using UnityEngine;

public class ZombieCharacter : EnemyCharacter
{
    public ZombieCharacter(EnemySprite sprite, string typ) : base(sprite, typ) { }

    public override void Behavior(GameObject gameObject)
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        Vector3 direction = GameManager.Instance.player.transform.position - gameObject.transform.position;
        float distance = direction.magnitude;

        if (distance < 2f)
        {
            
            if (controller.last_attack + 2f < Time.time)
            {
                controller.last_attack = Time.time;
                GameManager.Instance.playerController.player.hp.Damage(new Damage(5, Damage.Type.PHYSICAL));
            }
        }
        else
        {
            if (controller != null)
            {
                this.destination = GameManager.Instance.player.transform.position;
            }
        }
    }
}