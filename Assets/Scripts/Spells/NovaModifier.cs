using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class NovaModifier : ModifierSpell
{
    private float aoeDamage;
    private float aoeRadius;
    private Damage.Type damageType;
    
    public NovaModifier(float aoeDamage, float aoeRadius, Damage.Type damageType = Damage.Type.ARCANE)
    {
        this.aoeDamage = aoeDamage;
        this.aoeRadius = aoeRadius;
        this.damageType = damageType;
        this.name = "Nova";
        this.description = "Creates area-of-effect damage on hit.";
    }
    
    public NovaModifier(JObject obj) : base(obj)
    {
        this.aoeDamage = float.Parse(obj["aoe_damage"].ToString());
        this.aoeRadius = float.Parse(obj["aoe_radius"].ToString());
        string damageTypeStr = obj["damage_type"]?.ToString() ?? "arcane";
        this.damageType = Damage.TypeFromString(damageTypeStr);
    }
    
    public override Spell OnHit(Spell spell, Controller hitTarget)
    {
        // Get the position where the projectile hit
        // Assuming the Controller is attached to a GameObject
        if (hitTarget is MonoBehaviour monoBehaviour)
        {
            Vector3 hitPosition = monoBehaviour.transform.position;

            // Perform AOE damage at the hit location
            PerformAoEDamage(hitPosition, spell.team);
        }
        return spell;
    }

    private void PerformAoEDamage(Vector3 centerPosition, Hittable.Team sourceTeam)
    {
        // Use Physics2D.OverlapCircleAll to find all 2D colliders within the AOE radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(centerPosition, aoeRadius);

        foreach (Collider2D collider in hitColliders)
        {
            // Try to get the Controller component from the hit object
            Controller targetController = collider.GetComponent<Controller>();

            if (targetController == null)
                continue;

            // Skip if no character component
            if (targetController.character == null || targetController.character.hp == null)
                continue;

            // Skip if it\'s the same team (don\'t damage allies)
            if (targetController.character.hp.team == sourceTeam)
                continue;

            // Apply AOE damage
            targetController.character.hp.Damage(new Damage(aoeDamage, damageType));

            // Trigger damage event for visual feedback
            // Assuming the Controller is attached to a GameObject
            if (targetController is MonoBehaviour targetMonoBehaviour)
            {
                EventBus.Instance.TriggerPhysicalDamage(targetMonoBehaviour.transform.position, new Damage(aoeDamage, damageType), targetController.character.hp);
            }
        }
    }

    // For debugging - visualize the AOE radius in the scene view
    // This method requires the script to be a MonoBehaviour.
    // If NovaModifier is not a MonoBehaviour, this will not be called automatically.
    // Consider moving this to a MonoBehaviour that holds a NovaModifier if visualization is needed.
    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // transform.position would also need to be accessed carefully if this class is not a MonoBehaviour
        // For example, if this modifier is part of a spell attached to a character,
        // you might need to get the character's transform.
        // Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
    */
}