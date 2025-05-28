using UnityEngine;
using Newtonsoft.Json.Linq;

public class LifeLink : Relic
{
    private PlayerController currentPC;
    
    public override void JsonInit(JObject jsonObj)
    {
        base.JsonInit(jsonObj);
    }

    public override void Application(PlayerController pc)
    {
        currentPC = pc;

        // Listen to global damage dealt
        EventBus.Instance.OnMonsterDamaged += OnMonsterBeDealt;
    }

    private void OnMonsterBeDealt(Damage dmg, GameObject obj)
    {
        // Only trigger if player dealt damage to enemies
        if (currentPC == null)
            return;
        int currentHealthPercent = currentPC.player.hp.hp;
        currentHealthPercent += (int)this.amount;
        if (currentHealthPercent < currentPC.player.hp.max_hp)
        {
            currentPC.player.hp.hp = currentHealthPercent;
            currentPC.healthui.SetHealth(currentPC.player.hp);
        }
        Debug.Log($"Life Link: Healed {amount}, current HP: {currentHealthPercent}");
        
    }

    public override void OnRemoved()
    {
        EventBus.Instance.OnMonsterDamaged -= OnMonsterBeDealt;
    }
}