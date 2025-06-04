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
    private Vector3 lastDestination ;
    private float pathCooldown = 0.5f; // 每 0.5 秒最多寻路一次
    private float lastPathTime = 0f;

    
    //冷却时间防止每一帧寻路一次 定时部分
    public void setCoolDown(float t = 0.5f)
    {
        pathCooldown = t;
    }
    private bool CoolDown()
    {
        lastPathTime = Time.time;
        return (Time.time - lastPathTime > pathCooldown);
    }




    public EnemyMovement(EnemyController character, Tilemap tilemap, LayerMask wallMask)
    {
        this.enemy = character;
        this.collisionTilemap = tilemap;
        this.wallMask = wallMask;
        this.setCoolDown();
    }

    public void MoveTowards(Vector3 destination)
    {

        /*if (!CoolDown())
            return;*/
        if (Vector3.Distance(destination, this.enemy.gameObject.transform.position) < 0.8f)
            return; // arrive destination.

        lastDestination = destination;

        if ( (currentPath.Count == 0 || pathIndex >= currentPath.Count || Vector3.Distance(destination, lastDestination) > 0.5f))
        {
            
            currentPath = Pathfinder.FindPath(enemy.transform.position, destination, collisionTilemap, wallMask);
            Debug.Log("first refind c:"+ currentPath.Count +" index:"+ pathIndex);
            pathIndex = 0;
            lastDestination = destination; // record the current destination
        }

        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
            return;
        Debug.Log("bbb");
        Vector3 target = currentPath[pathIndex]; 
        Vector3 direction = (target - enemy.transform.position).normalized;

        //check volume
        if (Physics2D.OverlapCircle(target, 0.3f, wallMask))
        {
            // check the position reachable
            Debug.Log("sec refind c:" + currentPath.Count + " index:" + pathIndex);
            currentPath = Pathfinder.FindPath(enemy.transform.position, destination, collisionTilemap, wallMask);
            pathIndex = 0;
            return;
        }
        //Debug.Log("ccc");
        // check the wall corner
        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, direction, 0.4f, wallMask);
        if (hit.collider != null)
        {
            // If be block
            Debug.Log("wall refind c:" + currentPath.Count + " index:" + pathIndex);
            currentPath = Pathfinder.FindPath(enemy.transform.position, destination, collisionTilemap, wallMask);
            pathIndex = 0;
            return;
        }
        //Debug.Log("ddd");
        // if arrive the position, turn to next position
        if (Vector3.Distance(enemy.transform.position, target) < 0.1f)
        {
            pathIndex++;
        }
        else
        {
            // moving
            enemy.GetComponent<Unit>().movement = direction * enemy.enemy.final_speed;
        }
    }
}