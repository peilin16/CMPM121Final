using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;



public class Spell
{
    //spell data
    public float last_cast;
    public SpellCaster owner { get;  set; }
    public Hittable.Team team;
    public SpellData data { get; private set; }//负责存储基础数值的数据结构，请不要更改它内部的信息
    public List<ModifierSpell> modifierSpells = new List<ModifierSpell>();


    protected bool is_applicated = false;
    public int final_mana_cost { get; set; }


    public Vector3 where { get; set; }
    public Vector3 target { get; set; }
    public float final_damage { get; set; }
    public float final_cooldown { get; set; }
    public float final_secondary_damage { get; set; }
    public float final_speed { get; set; }
    public string final_trajectory { get; set; }
    public float final_life_time { get; set; }
    public float final_N_val { get; set; }
    public bool castModified = true;
    public bool onHitModified = true;
    public float spellPower { get; set; }



    public Spell(SpellCaster owner, SpellData data)
    {
        this.owner = owner;
        this.data = data;

        this.final_mana_cost = data.base_mana_cost;
        this.final_damage = data.base_damage;
        this.final_cooldown = data.base_cooldown;
        this.final_speed = data.projectile.base_speed;
        this.final_secondary_damage = data.base_secondary_damage;
        this.final_trajectory = data.projectile.trajectory;
        this.final_life_time = data.projectile.base_lifetime;
        this.final_N_val = data.N_value;
    }

    public string GetName()
    {
        return data.name;
    }

    public int GetManaCost()
    {
        return this.final_mana_cost;
    }
    public float GetSpeed()
    {
        return this.final_speed;
    }
    public float GetDamage()
    {
        return this.final_damage;
    }

    public float GetCooldown()
    {
        return this.final_cooldown;
    }

    public int GetIcon()
    {
        return data.icon;
    }
    public string GetDescription()
    {
        return data.description;
    }

    public float getSecondDamage()
    {
        return this.final_secondary_damage;
    }
    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }


    //如果需要更改base的数值请使用这个方法
    public void ResetCurrentBaseData(float power, int wave) {

        //reset damage
        data.base_damage = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.damage.amount, wave, power));
        data.base_mana_cost = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.mana_cost, wave, power));
        data.base_cooldown = RPNCalculator.EvaluateFloat(data.cooldown, wave, power);
        if (!string.IsNullOrEmpty(data.secondary_damage))
            data.base_secondary_damage = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.secondary_damage, wave, power));

        if (!string.IsNullOrEmpty(data.N))
            data.N_value = Mathf.FloorToInt(RPNCalculator.EvaluateFloat(data.N, wave, power));
        if (data.projectile != null)
        {
            data.projectile.base_speed = RPNCalculator.EvaluateFloat(data.projectile.speed, wave, power);
            if (!string.IsNullOrEmpty(data.projectile.lifetime))
                data.projectile.base_lifetime = RPNCalculator.EvaluateFloat(data.projectile.lifetime, wave, power);
        }
        if (data.secondary_projectile != null)
        {
            data.secondary_projectile.base_speed = RPNCalculator.EvaluateFloat(data.secondary_projectile.speed, wave, power);
            if (!string.IsNullOrEmpty(data.secondary_projectile.lifetime))
                data.secondary_projectile.base_lifetime = RPNCalculator.EvaluateFloat(data.secondary_projectile.lifetime, wave, power);
        }


        this.final_mana_cost = data.base_mana_cost;
        this.final_damage = data.base_damage;
        this.final_cooldown = data.base_cooldown;
        this.final_speed = data.projectile.base_speed;
        this.final_secondary_damage = data.base_secondary_damage;
        this.final_trajectory = data.projectile.trajectory;
        this.final_life_time = data.projectile.base_lifetime;
        this.final_N_val = data.N_value;

        this.applicateModify();
    }
    //更新所有的 修改器 重新调用一遍application方法
    public void applicateModify()
    {
        this.is_applicated = true;
        foreach (var modifier in modifierSpells)
            modifier.Application(this);
    }

    

    /// <summary>
    ///  spell cast method 
    /// </summary>
    /// <param name="where"></param>
    /// <param name="target"></param>
    /// <param name="team"></param>
    /// <param name="isModified"></param>
    /// <returns></returns>
    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, bool isModified = true)
    {

        this.team = team;
        if (this.is_applicated == false && isModified)
            applicateModify();
       
        if (isModified)
        {
            int i = 0;
            foreach (var modifier in modifierSpells)
            {
                modifier.Cast(this);//采用修改
                CoroutineManager.Instance.StartManagedCoroutine("Player_spell", modifier.name + i, modifier.CastWithCoroutine(this)); //如果有协程的话
                i += 1;

            }
        }
        
           
        int spriteIndex = int.Parse(data.projectile.sprite);
        float speed = this.final_speed;
        string traj = this.final_trajectory;
        float lifetime = this.final_life_time;


        if (!string.IsNullOrEmpty(data.projectile.lifetime))
        {
            GameManager.Instance.projectileManager.CreateProjectile(spriteIndex, traj, where, target - where, speed, OnHit, lifetime);
        }
        else
        {
            GameManager.Instance.projectileManager.CreateProjectile(spriteIndex, traj, where, target - where, speed, OnHit);
        }
        
        yield return new WaitForEndOfFrame();
    }


    public void ApplyFlatSpellpowerBoost(int bonus)
    {
        final_damage += bonus;
    }


    public virtual void OnHit(Controller other, Vector3 impact)
    {
        //Debug.Log("on hit");
        if (onHitModified)
        {
            int i = 0;
            foreach (var modifier in modifierSpells)
            {
                modifier.OnHit(this,other);// on hit change
                CoroutineManager.Instance.StartManagedCoroutine("Player_spell", modifier.name + i, modifier.OnHitWithCoroutine(this, other));
                i += 1;

            }
        }
        if (other.character.hp.team != team)
        {
            // Defaulting to arcane damage type, but can be extended to use data.damage.type
            foreach (var modifier in modifierSpells)
                modifier.OnHit(this, other);
            other.character.hp.Damage(new Damage(GetDamage(), Damage.Type.ARCANE));
        }
        if (data.secondary_projectile != null)
        {
            int count = data.N_value;
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

                GameManager.Instance.projectileManager.CreateProjectile(
                    int.Parse(data.secondary_projectile.sprite),
                    data.secondary_projectile.trajectory,
                    impact,
                    dir,
                    data.secondary_projectile.base_speed,
                    (Controller other, Vector3 _) =>
                    {
                        if (other.character.hp.team != team)
                            other.character.hp.Damage(new Damage(data.base_secondary_damage, Damage.Type.ARCANE));
                    },
                    data.secondary_projectile.base_lifetime
                );
            }
        }


    }



    public virtual void OnHitToWall(Controller other, Vector3 impact)
    {

    }
}
