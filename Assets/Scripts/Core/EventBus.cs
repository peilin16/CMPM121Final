using UnityEngine;
using System;

public class EventBus 
{
    //���лص���Ӧд��eventbus��
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    //player event
    public Action<Damage> OnPlayerDamaged;
    public Action<GameObject> OnPlayerDeath;
    public Action<PlayerController> PlayerCast; 

    // monster event
    public  Action<Damage, GameObject> OnMonsterDamaged; 
    public  Action<GameObject> OnMonsterDeath;
    public Action<Controller> SpellHitEnemy;

    public Action OnPlayerStandStill; 
    
    public float standStillTime { get; private set; }

    public void TriggerPlayerCast(PlayerController pc) => PlayerCast?.Invoke(pc);
    
    
    // spell collison 
    public Action<Controller> SpellCollision;
    public Action<Controller> SpellCollideToWall;
    //room
    public Action<Controller> CleanRoomAction;
    //Game Victory
    public Action<Controller> Victory;

    public void TriggerStandStill()
    {
        OnPlayerStandStill?.Invoke();
        standStillTime = 0f; 
    }

    public void TriggerSpellHitEnemy(Controller c)
    {
        SpellHitEnemy?.Invoke(c);
    }
    public void TriggerSpellCollision(Controller c)
    {
        SpellCollision?.Invoke(c);
    }
    public void TriggerSpellCollideToWall(Controller c)
    {
        SpellCollideToWall?.Invoke(c);
    }

    public void TriggerCleanRoomAction(Controller c)
    {
        CleanRoomAction?.Invoke(c);
    }
    public event Action<Vector3, Damage, Hittable> OnPhysicalDamage;

    public void TriggerPhysicalDamage(Vector3 position, Damage damage, Hittable target)
    {
        OnPhysicalDamage?.Invoke(position, damage, target);
    }

    public void TriggerPlayerDamaged(Damage damage)
    {
        OnPlayerDamaged?.Invoke(damage);
    }

    public void TriggerOnMonsterDamaged(Damage damage, GameObject monster)
    {
        OnMonsterDamaged?.Invoke(damage,monster);
    }
    public void TriggerOnMonsterDeath(GameObject monster)
    {
        OnMonsterDeath?.Invoke(monster);
    }
   public void TriggerOnPlayerDeath(GameObject player)
    {
        
       OnPlayerDeath?.Invoke(player);
    }


}
