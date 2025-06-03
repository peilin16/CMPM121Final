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
    public Action<Damage> OnPlayerDamaged;//��������¼�
    public Action<GameObject> OnPlayerDeath;//��������¼�
    public Action<PlayerController> PlayerCast; //���������¼�

    // monster event
    public  Action<Damage, GameObject> OnMonsterDamaged; //���������¼�
    public  Action<GameObject> OnMonsterDeath; // ���������¼�
    public Action<Controller> SpellHitEnemy; //�������е��˵��¼�
    // �޸� OnPlayerStandStill ���壨�Ƴ� PlayerController ������
    public Action OnPlayerStandStill;  // ��Ҿ�ֹ�¼�

    // ���Ӿ�ֹʱ���ֶΣ�������뱣����
    public float standStillTime { get; private set; }

    public void TriggerPlayerCast(PlayerController pc) => PlayerCast?.Invoke(pc);
    
    
    //������ײ�¼�
    // spell collison 
    public Action<Controller> SpellCollision;
    public Action<Controller> SpellCollideToWall;//����ײǽ�¼�

    // ���Ӵ�������
    public void TriggerStandStill()
    {
        OnPlayerStandStill?.Invoke();
        standStillTime = 0f; // ���������ü�ʱ
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

    // ͨ�������˺��¼�������ԭʼ��ƣ�
    public event Action<Vector3, Damage, Hittable> OnPhysicalDamage;

    public void TriggerPhysicalDamage(Vector3 position, Damage damage, Hittable target)
    {
        OnPhysicalDamage?.Invoke(position, damage, target);
    }
    // �� EventBus ��������
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
        if(GameManager.Instance.state == GameManager.GameState.INWAVE)
            OnPlayerDeath?.Invoke(player);
    }


}
