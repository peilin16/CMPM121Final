using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
public class SpeedAmpModifier : ModifierSpell
{
    private float speedMultiplier;
    private float finalSpeed;
    public SpeedAmpModifier(float mult)
    {
        this.speedMultiplier = mult;
    }
    public SpeedAmpModifier(JObject obj):base(obj)
    {
        this.speedMultiplier = float.Parse(obj["speed_multiplier"].ToString());
    }
    public override Spell Application(Spell spell)
    {
        spell.final_speed *= speedMultiplier;
        if (spell.data.secondary_projectile != null)
            spell.data.secondary_projectile.base_speed *= speedMultiplier;
        return spell;
    }

    
}