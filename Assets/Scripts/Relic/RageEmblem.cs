using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class RageEmblem : Relic
{
    private PlayerController boundPC;
    private bool isBuffActive = false;
    private float originalSpellPower;
    private float newSpellPower;
    public override void JsonInit(JObject jsonObj)
    {
        base.JsonInit(jsonObj);
    }

    public override void Application(PlayerController pc)
    {
        boundPC = pc;
        EventBus.Instance.OnPlayerDamaged += OnPlayerDamaged;
        newSpellPower = boundPC.player.spellcaster.spellPower * (amount / 100f);
    }

    private void OnPlayerDamaged(Damage dmg)
    {
        if (isBuffActive || boundPC?.player?.spellcaster == null)
            return;

        isBuffActive = true;
        originalSpellPower = boundPC.player.spellcaster.spellPower;

        float bonus = originalSpellPower ;
        boundPC.player.spellcaster.spellPower = newSpellPower;
        //boundPC.player.spellcaster.resetSpellsData();

        Debug.Log($"Rage Emblem activated: +{bonus} damage boost");

        CoroutineManager.Instance.StartManagedCoroutine(
            "RageEmblem",
            boundPC.Controller_ID.ToString(),
            ResetAfterDuration(3f)
        );
    }

    private IEnumerator ResetAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (boundPC?.player?.spellcaster != null)
        {
            boundPC.player.spellcaster.spellPower = originalSpellPower;
            Debug.Log("Rage Emblem expired: Spellpower reset.");
        }

        isBuffActive = false;
    }

    public override void OnRemoved()
    {
        EventBus.Instance.OnPlayerDamaged -= OnPlayerDamaged;
        isBuffActive = false;
    }
}