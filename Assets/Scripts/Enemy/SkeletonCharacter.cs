using UnityEngine;

public class SkeletonCharacter : EnemyCharacter
{
    private EnemyCaster caster;
    [SerializeField] private LayerMask obstacleLayerMask = 0; // Start with no obstacles for testing
    
    public SkeletonCharacter() : base() 
    {
        stopDistance = 1.0f; // Skeleton keeps closer distance than warlock
    }
    
    public SkeletonCharacter(EnemySprite sprite, string type) : base(sprite, type) 
    {
        stopDistance = 1.0f; // Skeleton keeps closer distance than warlock
    }
    
    public override EnemyCharacter Clone()
    {
        return new SkeletonCharacter(this.enemySprite, this.type)
        {
            final_healthly = this.final_healthly,
            final_damage = this.final_damage,
            final_speed = this.final_speed
        };
    }
      public override void InitController(EnemyController c)
    {
        base.InitController(c);
        
        // Initialize spell casting for skeleton
        caster = new EnemyCaster(Hittable.Team.MONSTERS, 8f, 3f); // 8 range, 3s cooldown
        caster.AddSpell("arcane_bolt"); // Use existing spells from the database
        caster.AddSpell("magic_missile");
    }
    
    public override void Behavior(GameObject gameObject)
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        Vector3 direction = GameManager.Instance.player.transform.position - gameObject.transform.position;
        float distance = direction.magnitude;

        // Skeleton tries to maintain closer casting distance than warlock
        if (distance <= castRange && distance > stopDistance)
        {
            // In optimal range - move slowly around the player
            MoveRandomlyAroundPlayer(gameObject);
        }
        else if (distance <= stopDistance)
        {
            // Too close - back away slightly
            Vector3 awayDirection = -direction.normalized;
            this.destination = gameObject.transform.position + awayDirection * 1.5f;
        }
        else
        {
            // Too far - move closer to casting range
            this.destination = GameManager.Instance.player.transform.position;
        }
    }
    
    private float lastRandomMoveTime = 0f;
    private float randomMoveInterval = 3f; // Change direction slower than warlock (every 3 seconds)
    
    private void MoveRandomlyAroundPlayer(GameObject gameObject)
    {
        // Only change direction every few seconds - slower than warlock for more predictable movement
        if (Time.time - lastRandomMoveTime >= randomMoveInterval)
        {
            Vector3 playerPosition = GameManager.Instance.player.transform.position;
            Vector3 currentPosition = gameObject.transform.position;
            
            // Generate a random angle
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            // Calculate desired distance (closer range than warlock)
            float desiredDistance = Random.Range(stopDistance + 0.5f, castRange - 0.5f);
            
            // Calculate new position around the player
            Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
            Vector3 newDestination = playerPosition + randomDirection * desiredDistance;
            
            this.destination = newDestination;
            lastRandomMoveTime = Time.time;
            
            // Vary the interval for more natural movement, but slower than warlock
            randomMoveInterval = Random.Range(2.5f, 4f);
        }
    }
    
    private float castRange => caster?.castRange ?? 8f;
    
    public override void UpdateBehavior(Vector3 playerPosition)
    {
        base.UpdateBehavior(playerPosition);
        
        // Try to cast spells at the player
        if (caster != null && caster.CanCast())
        {
            caster.TryCastAtTarget(this.gameObject.transform.position, playerPosition, obstacleLayerMask);
        }
    }
}
