using UnityEngine;
using System;
using System.Collections;

public class ProjectileController : MonoBehaviour
{
    public float lifetime;
    public event Action<Controller, Vector3> OnHit;
    public ProjectileMovement movement;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement.Movement(transform);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("projectile")) {
            
            return;
                
        }
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
        EnemyController ec = collision.gameObject.GetComponent<EnemyController>();
        if (collision.gameObject.CompareTag("unit"))
        {
            
            if (ec != null)
            {
                //我会在这里触发两次事件请通过controller的子类型区分
                EventBus.Instance.TriggerSpellHitEnemy(pc);
                //EventBus.Instance.TriggerSpellHitEnemy(ec);
                Debug.Log("project tile collison to enemy");
                OnHit(ec, transform.position);
            }
            else
            {
                Debug.Log("project tile collison to wall");
                
                if (pc != null)
                {
                    OnHit(pc, transform.position);
                }
            }

        }
        else 
        {
            // Trigger SpellCollideToWall
            EventBus.Instance.TriggerSpellCollideToWall(pc);

            //Debug.Log("project tile collison to wall");
        }
        EventBus.Instance.TriggerSpellCollision(pc);
        //Debug.Log("project tile collison");
        Destroy(gameObject);
    }

    public void SetLifetime(float lifetime)
    {
        StartCoroutine(Expire(lifetime));
    }

    IEnumerator Expire(float lifetime)
    {
        
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
