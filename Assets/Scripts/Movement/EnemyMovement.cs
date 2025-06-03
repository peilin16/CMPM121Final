using UnityEngine;
using System.Collections.Generic;

//请把所有敌人相关的移动逻辑写在该class下
public class EnemyMovement
{
    private EnemyCharacter character;
    private List<Vector3> path;
    private int currentPathIndex = 0;

    public EnemyMovement(EnemyCharacter character)
    {
        this.character = character;
    }

    public void MoveTowardsPlayer()
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        Vector3 direction = GameManager.Instance.player.transform.position - character.gameObject.transform.position;
        float distance = direction.magnitude;

        if (distance >= 2f)
        {
            character.gameObject.GetComponent<Unit>().movement = direction.normalized * character.final_speed;
        }
    }

    //A* 寻路算法 测试中
    public void MoveTowards(Vector3 destination)
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            path = FindPath(transform.position, destination);
            currentPathIndex = 0;
        }

        if (path != null && currentPathIndex < path.Count)
        {
            Vector3 targetPos = path[currentPathIndex];
            Vector3 direction = (targetPos - transform.position).normalized;
            character.gameObject.GetComponent<Unit>().movement = direction * character.final_speed;

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                currentPathIndex++;
            }
        }
    }

    private List<Vector3> FindPath(Vector3 start, Vector3 goal)
    {
        Vector2Int startGrid = WorldToGrid(start);
        Vector2Int goalGrid = WorldToGrid(goal);

        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        openSet.Enqueue(startGrid, 0);
        gScore[startGrid] = 0;

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();
            if (current == goalGrid)
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet.Add(current);
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    float fScore = tentativeGScore + Heuristic(neighbor, goalGrid);
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }

        return null;
    }

    private List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector3> totalPath = new List<Vector3>();
        totalPath.Add(GridToWorld(current));
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, GridToWorld(current));
        }
        return totalPath;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int node)
    {
        Vector2Int[] directions = new Vector2Int[] {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = node + dir;
            if (!IsBlocked(neighbor))
                yield return neighbor;
        }
    }

    private bool IsBlocked(Vector2Int cell)
    {
        Vector3 worldPos = GridToWorld(cell);
        Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.3f, LayerMask.GetMask("Wall"));
        return hit != null;
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cellPos = GameManager.Instance.tilemap.WorldToCell(worldPos);
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    private Vector3 GridToWorld(Vector2Int gridPos)
    {
        return GameManager.Instance.tilemap.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }


}
