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

    




    
    public EnemyMovement(EnemyController character, Tilemap tilemap, LayerMask wallMask)
    {
        this.enemy = character;
        this.collisionTilemap = tilemap;
        this.wallMask = wallMask;
        //this.setCoolDown();
    }

    public void setCoolDown(float c) {
        pathCooldown = c;
    }

    //cool down
    private bool IsCooldownOver()
    {
        return (Time.time - lastPathTime > pathCooldown);
    }

    private void ResetCooldown()
    {
        lastPathTime = Time.time;
    }

    public void MoveTowards(Vector3 destination, float stopDistance = 0.7f)
    {

        /*if (!CoolDown())
            return;*/
        /*if ()
            return; // arrive destination.*/


        //Debug.Log("D: "+Vector3.Distance(enemy.transform.position, destination));
        if (Vector3.Distance(enemy.transform.position, destination) <= stopDistance)
        {
            enemy.GetComponent<Unit>().movement = Vector2.zero;
            //Debug.Log("stop");
            return;
        }




        bool needNewPath = (lastDestination != destination && IsCooldownOver());

        if (needNewPath || currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            currentPath = Pathfinder.FindPath(enemy.transform.position, destination, collisionTilemap, wallMask);
            pathIndex = 0;
            lastDestination = destination;
            ResetCooldown();
        }

        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
            return;
        //Debug.Log("bbb");
        Vector3 target = currentPath[pathIndex]; 
        Vector3 direction = (target - enemy.transform.position).normalized;

        //check volume
        if (Physics2D.OverlapCircle(target, 0.3f, wallMask))
        {
            // check the position reachable
            //Debug.Log("sec refind c:" + currentPath.Count + " index:" + pathIndex);
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
            //Debug.Log("wall refind c:" + currentPath.Count + " index:" + pathIndex);
            currentPath = Pathfinder.FindPath(enemy.transform.position, destination, collisionTilemap, wallMask);
            pathIndex = 0;
            return;
        }
        //Debug.Log("ddd");
        // if arrive the position, turn to next position
        if (Vector3.Distance(enemy.transform.position, target) < 0.3f)
        {
            pathIndex++;
            // If final path
            /*if (pathIndex >= currentPath.Count)
            {
                enemy.GetComponent<Unit>().movement = Vector2.zero;
                return;
            }*/
        }
        else
        {
            // moving
            enemy.GetComponent<Unit>().movement = direction * enemy.enemy.final_speed;
            //Debug.Log(" index:" + pathIndex+ " " + currentPath[pathIndex] + " " + currentPath[pathIndex - 1] +" "+ destination);
        }
        //Debug.Log("is Working");
        /*if (Vector3.Distance(enemy.transform.position, destination) < 0.5f)
        {
            enemy.GetComponent<Unit>().movement = Vector2.zero;
            return;
        }*/
    }
}