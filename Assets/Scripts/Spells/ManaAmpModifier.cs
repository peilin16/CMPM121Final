using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
public class ManaAmpModifier : ModifierSpell
{
    private float manaMultiplier;

    public ManaAmpModifier(float multiplier)
    {
        this.manaMultiplier = multiplier;
    }
    public ManaAmpModifier(JObject obj):base(obj)
    {
        this.manaMultiplier = float.Parse(obj["mana_multiplier"].ToString());
    }

    public override Spell Application(Spell spell)
    {
        spell.final_mana_cost = Mathf.RoundToInt(spell.final_mana_cost * manaMultiplier);
        return spell;
    }
}
