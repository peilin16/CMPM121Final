using UnityEngine;

public class WarlockCharacter : EnemyCharacter
{
    private EnemyCaster caster;
    [SerializeField] private LayerMask obstacleLayerMask = 0; // Start with no obstacles for testing
    
    public WarlockCharacter() : base() 
    {
        stopDistance = 1.5f; // Warlock keeps more distance for powerful spells
    }
    
    public WarlockCharacter(EnemySprite sprite, string type) : base(sprite, type) 
    {
        stopDistance = 1.5f; // Warlock keeps more distance for powerful spells
    }
    
    public override EnemyCharacter Clone()
    {
        return new WarlockCharacter(this.enemySprite, this.type)
        {
            final_healthly = this.final_healthly,
            final_damage = this.final_damage,
            final_speed = this.final_speed
        };
    }      public override void InitController(EnemyController c)
    {
        base.InitController(c);
        
        // Warlock has more powerful spells with 10 second cooldown
        caster = new EnemyCaster(Hittable.Team.MONSTERS, 12f, 5f); // 12 range, 5s cooldown
        caster.AddSpell("arcane_bolt");
        caster.AddSpell("arcane_blast"); // More powerful spell
        caster.AddSpell("arcane_nova"); // AOE spell
    }
      public override void Behavior(GameObject gameObject)
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        Vector3 direction = GameManager.Instance.player.transform.position - gameObject.transform.position;
        float distance = direction.magnitude;

        // Warlock tries to maintain optimal casting distance
        if (distance <= castRange && distance > stopDistance)
        {
            // In optimal range - move randomly around the player
            MoveRandomlyAroundPlayer(gameObject);
        }
        else if (distance <= stopDistance)
        {
            // Too close - back away slightly
            Vector3 awayDirection = -direction.normalized;
            this.destination = gameObject.transform.position + awayDirection * 2f;
        }
        else
        {
            // Too far - move closer to casting range
            this.destination = GameManager.Instance.player.transform.position;
        }
    }
    
    private float lastRandomMoveTime = 0f;
    private float randomMoveInterval = 2f; // Change direction every 2 seconds
    
    private void MoveRandomlyAroundPlayer(GameObject gameObject)
    {
        // Only change direction every few seconds to avoid erratic movement
        if (Time.time - lastRandomMoveTime >= randomMoveInterval)
        {
            Vector3 playerPosition = GameManager.Instance.player.transform.position;
            Vector3 currentPosition = gameObject.transform.position;
            
            // Generate a random angle
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            
            // Calculate desired distance (between stopDistance and castRange)
            float desiredDistance = Random.Range(stopDistance + 1f, castRange - 1f);
            
            // Calculate new position around the player
            Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f);
            Vector3 newDestination = playerPosition + randomDirection * desiredDistance;
            
            this.destination = newDestination;
            lastRandomMoveTime = Time.time;
            
            // Vary the interval slightly for more natural movement
            randomMoveInterval = Random.Range(1.5f, 3f);
        }
    }
    
    private float castRange => caster?.castRange ?? 12f;      public override void UpdateBehavior(Vector3 playerPosition)
    {
        base.UpdateBehavior(playerPosition);
        
        if (caster != null)
        {
            if (caster.CanCast())
            {
                caster.TryCastAtTarget(this.gameObject.transform.position, playerPosition, obstacleLayerMask);
            }
        }
    }
}
