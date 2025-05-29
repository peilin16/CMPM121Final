using System.Collections;  // <== THIS is required for IEnumerator
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public abstract class ModifierSpell
{
    public string name;
    public string description;
    public bool isActive = true;
    public bool one_time = false;
    public ModifierSpell() { }
    public ModifierSpell(JObject obj) {
        name = obj["name"]?.ToString();
        description = obj["description"]?.ToString();
    }
    
    public virtual Spell Cast(Spell spell) { return spell; }//投射时执行的方法
    public virtual Spell OnHit(Spell spell, Controller other) { return spell; }// 咒语击中敌人时执行的方法 
    public virtual Spell OnHitToWall(Spell spell, Controller other) { return spell; }// 咒语击中墙面时执行的方法 
    public virtual Spell Application(Spell spell) { return spell; } //在需要时执行一次的方法，例如更改一次数值
    public virtual IEnumerator CastWithCoroutine(Spell spell)//投射时执行的协程
    {
        yield break; 
    }
    public virtual IEnumerator OnHitWithCoroutine(Spell spell, Controller other)//咒语击中敌人时执行的的协程
    {
        yield break;
    }

}