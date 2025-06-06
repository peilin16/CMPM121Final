using UnityEngine;
using System.Collections;

public class ArcaneNova : Spell
{    public ArcaneNova(SpellCaster owner, SpellData data) : base(owner, data)
    {
        // Add the nova modifier by default
        NovaModifier novaModifier = new NovaModifier(data.base_damage * 0.8f, 3f, Damage.Type.ARCANE);
        this.modifierSpells.Add(novaModifier);

        // Keep the original speed for now to test if projectile appears
        // this.final_speed *= 0.5f;
    }    public override IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team, bool isModified = true)
    {
        // Set the where and target fields like the base spell does
        this.where = where;
        this.target = target;
        this.team = team;

        // Apply modifiers if needed
        if (this.is_applicated == false && isModified)
            applicateModify();

        if (isModified)
        {
            int i = 0;
            foreach (var modifier in modifierSpells)
            {
                modifier.Cast(this);
                CoroutineManager.Instance.StartManagedCoroutine("Player_spell", modifier.name + i, modifier.CastWithCoroutine(this));
                i += 1;
            }
        }        // Use the same direction calculation as the base spell
        Vector3 direction = target - where;

        // Create a single projectile with the nova effect - use OnHit method directly like base spell
        // Match the base spell's logic for lifetime handling
        if (!string.IsNullOrEmpty(data.projectile.lifetime))
        {
            GameManager.Instance.projectileManager.CreateProjectile(
                int.Parse(data.projectile.sprite),
                final_trajectory,
                where,
                direction,
                final_speed,
                OnHit,
                final_life_time
            );
        }
        else
        {
            GameManager.Instance.projectileManager.CreateProjectile(
                int.Parse(data.projectile.sprite),
                final_trajectory,
                where,
                direction,
                final_speed,
                OnHit
            );
        }

        yield return new WaitForEndOfFrame();
    }
}
