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
        if (other != null && other.character != null && other.character.movement != null)
        {
            string group = "enemy_slow";
            string id = other.GetInstanceID().ToString(); // Unique ID per enemy

            CoroutineManager.Instance.StartManagedCoroutine(group, id, ApplySlow(other));
        }
        return spell;
    }

    private IEnumerator ApplySlow(Controller target)
    {
        if (target.character != null)
        {
            int originalSpeed = target.character.speed;
            target.character.speed = (int)(target.character.speed * slowFactor);

            yield return new WaitForSeconds(slowDuration);

            if (target.character != null && target.character.movement != null)
                target.character.speed = originalSpeed;
        }
    }
}