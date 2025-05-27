using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
public class WaverModifier : ModifierSpell
{
    private float damageMultiplier;
    private int manaAdder;

    public WaverModifier(float damageMult, int manaAdd)
    {
        this.damageMultiplier = damageMult;
        this.manaAdder = manaAdd;
    }
    public WaverModifier(JObject obj) : base(obj)
    {
        this.damageMultiplier = float.Parse(obj["damage_multiplier"].ToString());
        this.manaAdder = int.Parse(obj["mana_adder"].ToString());
    }
    public override Spell Application(Spell spell)
    {
        spell.final_damage = Mathf.RoundToInt(spell.final_damage * damageMultiplier);
        spell.final_mana_cost += manaAdder;
        spell.final_trajectory = "wave";
        return spell;
    }
}
