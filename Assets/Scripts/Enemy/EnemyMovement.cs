// EnemyMovement.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyMovement
{
    private EnemyController enemy;
    private List<Vector3> currentPath = new List<Vector3>();
    private int pathIndex = 0;
    private Tilemap collisionTilemap;
    private LayerMask wallMask;

    public EnemyMovement(EnemyController character, Tilemap tilemap, LayerMask wallMask)
    {
        this.enemy = character;
        this.collisionTilemap = tilemap;
        this.wallMask = wallMask;
    }

    public void MoveTowards(Vector3 destination)
    {
        if (currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            currentPath = Pathfinder.FindPath(enemy.transform.position, destination, collisionTilemap, wallMask);
            pathIndex = 0;
        }

        if (currentPath != null && pathIndex < currentPath.Count)
        {
            Vector3 target = currentPath[pathIndex];
            Vector2 dir = (target - enemy.transform.position).normalized;
            enemy.gameObject.GetComponent<Unit>().movement = dir * enemy.characterData.final_speed;

            if (Vector3.Distance(enemy.transform.position, target) < 0.1f)
            {
                pathIndex++;
            }
        }
    }
}