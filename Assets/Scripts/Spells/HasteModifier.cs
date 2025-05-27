using UnityEngine;
using Newtonsoft.Json.Linq;

public class HasteModifier : ModifierSpell
{
    private float cooldownMultiplier;
    private float manaMultiplier;

    public HasteModifier(float cooldownMult, float manaMult)
    {
        this.cooldownMultiplier = cooldownMult;
        this.manaMultiplier = manaMult;
    }

    public HasteModifier(JObject obj) : base(obj)
    {
        this.cooldownMultiplier = float.Parse(obj["cooldown_multiplier"].ToString());
        this.manaMultiplier = float.Parse(obj["mana_multiplier"].ToString());
    }

    public override Spell Application(Spell spell)
    {
        spell.final_cooldown *= cooldownMultiplier;
        spell.final_mana_cost = Mathf.RoundToInt(spell.final_mana_cost * manaMultiplier);
        return spell;
    }
}
