using UnityEngine;
using System.Collections;

public class MagicBread : Relic
{
    private bool pendingDiscount = false;
    private PlayerController player;
    public override void Application(PlayerController pc)
    {
        player = pc;
        EventBus.Instance.SpellCollideToWall += OnSpellMiss;
        EventBus.Instance.PlayerCast += OnPlayerCast;
        EventBus.Instance.SpellHitEnemy += OnSpellHit;
    }



    private void OnSpellMiss(Controller controller)
    {
        Debug.Log($"Magic Bread trigger");
        //if (controller is PlayerController playerController)
        //{
            
            pendingDiscount = true;
        //}
    }

    private void OnPlayerCast(PlayerController pc)
    {
        if (pendingDiscount && pc == player)
        {
            //Debug.Log($"Magic Bread trigger reduce {amount}");
            foreach (var spell in pc.player.spellcaster.spells)
            {
                
                spell.final_mana_cost = spell.final_mana_cost - Mathf.RoundToInt(amount);
                //Debug.Log($"Magic Bread trigger reduce { spell.final_mana_cost}");
            }
            pendingDiscount = false;
        }
    }
    private void OnSpellHit(Controller controller)
    {
        // 检查是否为 PlayerController
        if (controller is PlayerController playerController)
        {
            // 确保 player 和 spellcaster 不为 null
            if (playerController.player != null && playerController.player.spellcaster != null)
            {
                // 遍历所有 spells 并调整 mana_cost
                foreach (var spell in playerController.player.spellcaster.spells)
                {
                    spell.final_mana_cost =  spell.final_mana_cost + Mathf.RoundToInt(amount);
                }
            }
            Debug.Log($"Magic Bread in active");
            pendingDiscount = false; // 命中后取消效果
        }
    }
    public override void OnRemoved()
    {
        EventBus.Instance.SpellCollideToWall -= OnSpellMiss;
        EventBus.Instance.PlayerCast -= OnPlayerCast;
        EventBus.Instance.SpellHitEnemy -= OnSpellHit;
    }
}