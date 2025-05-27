
// DoublerModifier.cs
using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public class DoublerModifier : ModifierSpell
{
    private float delay;
    private float manaMultiplier;
    private float cooldownMultiplier;

    public DoublerModifier(float delay, float manaMult, float cooldownMult)
    {
        this.delay = delay;
        this.manaMultiplier = manaMult;
        this.cooldownMultiplier = cooldownMult;
    }
    public DoublerModifier(JObject obj):base(obj)
    {
        this.delay = float.Parse(obj["delay"].ToString());
        this.manaMultiplier = float.Parse(obj["mana_multiplier"].ToString());
        this.cooldownMultiplier = float.Parse(obj["cooldown_multiplier"].ToString());
    }

    public override Spell Application(Spell spell)
    {
        spell.final_mana_cost = Mathf.RoundToInt(spell.final_mana_cost * manaMultiplier);
        spell.final_cooldown *= cooldownMultiplier;
        return spell;
    }



    public override IEnumerator CastWithCoroutine(Spell spell)
    {
        yield return new WaitForSeconds(delay);
 
        yield return spell.Cast(spell.where, spell.target, spell.team,false);
    }
    
}
