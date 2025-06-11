using UnityEngine;
using System.Collections.Generic;

public class EnemyCaster
{
    private List<string> availableSpells;
    public float castRange = 10f;
    public float castCooldown = 2f;
    private float lastCastTime;
    private Hittable.Team team;
    
    public EnemyCaster(Hittable.Team team, float range = 10f, float cooldown = 2f)
    {
        this.team = team;
        availableSpells = new List<string>();
        castRange = range;
        castCooldown = cooldown;
        lastCastTime = -cooldown; // Allow immediate first cast
    }
    
    public void AddSpell(string spellKey)
    {
        if (!availableSpells.Contains(spellKey))
        {
            availableSpells.Add(spellKey);
        }
    }
    
    public bool CanCast()
    {
        return Time.time - lastCastTime >= castCooldown && availableSpells.Count > 0;
    }
    
    public bool HasLineOfSight(Vector3 startPosition, Vector3 targetPosition, LayerMask obstacleLayerMask)
    {
        Vector3 direction = (targetPosition - startPosition).normalized;
        float distance = Vector3.Distance(startPosition, targetPosition);
        
        // Raycast to check for obstacles
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, distance, obstacleLayerMask);
        return hit.collider == null; // No obstacles in the way
    }      public void TryCastAtTarget(Vector3 position, Vector3 targetPosition, LayerMask obstacleLayerMask)
    {
        if (!CanCast()) return;
        
        float distance = Vector3.Distance(position, targetPosition);
        if (distance > castRange) return;
        
        bool hasLOS = HasLineOfSight(position, targetPosition, obstacleLayerMask);
        
        // Debug visualization
        DrawLineOfSight(position, targetPosition, hasLOS);
        
        if (!hasLOS) return;
        
        // Create a temporary spell caster with high mana (enemies ignore mana costs)
        SpellCaster tempCaster = new SpellCaster(999999, 0, team, 10f);
        
        // Get a random spell from available spells
        string spellKey = availableSpells[Random.Range(0, availableSpells.Count)];
        Spell spell = GameManager.Instance.spellBuilder.Build(tempCaster, spellKey);
        
        // Cast the spell
        CoroutineManager.Instance.StartManagedCoroutine("Enemy_spell", "cast", 
            spell.Cast(position, targetPosition, team));
        
        lastCastTime = Time.time;
    }
    
    // Optional: Visual line of sight debugging
    public void DrawLineOfSight(Vector3 startPosition, Vector3 targetPosition, bool hasLOS)
    {
        if (Application.isEditor)
        {
            Color lineColor = hasLOS ? Color.green : Color.red;
            Debug.DrawLine(startPosition, targetPosition, lineColor, 0.1f);
        }
    }
}
