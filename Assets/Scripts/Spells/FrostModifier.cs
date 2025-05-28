using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;

public class FrostModifier : ModifierSpell
{
    private float slowFactor;
    private float slowDuration;
    
    public FrostModifier(float slowFactor, float slowDuration)
    {
        this.slowFactor = slowFactor;
        this.slowDuration = slowDuration;
        this.name = "Frost";
        this.description = "Slows enemies on hit.";
    }

    public FrostModifier(JObject obj) : base(obj)
    {
        this.slowFactor = float.Parse(obj["slow_factor"].ToString());
        this.slowDuration = float.Parse(obj["slow_duration"].ToString());
    }

    public override Spell OnHit(Spell spell, Controller other)
    {
        if (other != null && other.character != null && other.character != null)
        {
            string group = "enemy_slow";
            string id = other.Controller_ID.ToString(); // Unique ID per enemy

            CoroutineManager.Instance.StartManagedCoroutine(group, id, ApplySlow(other));
        }
        return spell;
    }

    private IEnumerator ApplySlow(Controller target)
    {
        if (target.character == null || target.character == null)
            yield break;

        float originalSpeed = target.character.speed;
        target.character.speed *= (int)slowFactor;

        yield return new WaitForSeconds(slowDuration);

        // Only reset if the movement still exists
        if (target.character != null && target.character != null)
            target.character.speed = (int)originalSpeed;
    }
}