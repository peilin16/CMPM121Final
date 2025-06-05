// Pathfinder.cs
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;


public static class Pathfinder
{
    private class Node
    {
        public Vector3 worldPos;
        public Vector3Int cellPos;
        public float gCost;
        public float hCost;
        public float fCost => gCost + hCost;
        public Node parent;

        public Node(Vector3Int cellPos, Vector3 worldPos)
        {
            this.cellPos = cellPos;
            this.worldPos = worldPos;
        }
    }

    public static List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld, Tilemap tilemap, LayerMask wallMask)
    {
        Vector3Int startCell = tilemap.WorldToCell(startWorld);
        Vector3Int endCell = tilemap.WorldToCell(endWorld);

        Dictionary<Vector3Int, Node> openSet = new Dictionary<Vector3Int, Node>();
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        Node startNode = new Node(startCell, tilemap.GetCellCenterWorld(startCell));
        startNode.gCost = 0;
        startNode.hCost = Vector3Int.Distance(startCell, endCell);
        openSet[startCell] = startNode;

        while (openSet.Count > 0)
        {
            Node current = null;
            foreach (var n in openSet.Values)
            {
                if (current == null || n.fCost < current.fCost)
                    current = n;
            }

            if (current.cellPos == endCell)
                return ReconstructPath(current);

            openSet.Remove(current.cellPos);
            closedSet.Add(current.cellPos);

            foreach (var offset in GetNeighbors())
            {
                Vector3Int neighborPos = current.cellPos + offset;
                if (closedSet.Contains(neighborPos))
                    continue;

                Vector3 world = tilemap.GetCellCenterWorld(neighborPos);
                /*if (Physics2D.OverlapCircle(world, 0.3f, wallMask)) // 
                    continue;*/
                if (IsUnwalkable(world, wallMask))
                    continue;
                float tentativeG = current.gCost + Vector3Int.Distance(current.cellPos, neighborPos);

                if (!openSet.TryGetValue(neighborPos, out Node neighbor))
                {
                    neighbor = new Node(neighborPos, world);
                    openSet[neighborPos] = neighbor;
                }
                else if (tentativeG >= neighbor.gCost)
                {
                    continue;
                }

                neighbor.parent = current;
                neighbor.gCost = tentativeG;
                neighbor.hCost = Vector3Int.Distance(neighborPos, endCell);
            }
        }

        return new List<Vector3>(); // No path
    }

    // check  passable
    private static bool IsUnwalkable(Vector3 worldPos, LayerMask wallMask)
    {
        return Physics2D.OverlapCircle(worldPos, 0.6f, wallMask);
    }


    private static List<Vector3> ReconstructPath(Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node current = endNode;
        while (current != null)
        {
            
            path.Add(current.worldPos);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    private static List<Vector3Int> GetNeighbors()
    {
        return new List<Vector3Int>
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.up + Vector3Int.left,
            Vector3Int.up + Vector3Int.right,
            Vector3Int.down + Vector3Int.left,
            Vector3Int.down + Vector3Int.right
        };
    }
}
