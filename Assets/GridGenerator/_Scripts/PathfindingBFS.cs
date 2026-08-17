using System.Collections.Generic;
using UnityEngine;

public class PathfindingBFS : MonoBehaviour
{
    public static PathfindingBFS Instance;
    
    public Dictionary<Vector2Int, HexInfos> _hexes = new Dictionary<Vector2Int, HexInfos>();

/*    public Material outlineMat;
    public Material holderMat;
    public Material selectedMat;*/

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private static readonly Vector2Int[] _evenOffsets =
    {
        new Vector2Int(-1, 0), new Vector2Int(1, 0),
        new Vector2Int(-1, 1), new Vector2Int(0, 1),
        new Vector2Int(-1, -1), new Vector2Int(0, -1)
    };

    private static readonly Vector2Int[] _oddOffsets =
    {
        new Vector2Int(-1, 0), new Vector2Int(1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

    public List<Vector2Int> GetNeighbors(Vector2Int coord)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] offsets = coord.y % 2 == 0 ? _evenOffsets : _oddOffsets;

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int offsetCoord = coord + offset;
            if(_hexes.ContainsKey(offsetCoord))
                neighbors.Add(offsetCoord);
        }
        return neighbors;
    }

    public List<Vector2Int> GetNeighbors(Vector2Int coord, int radius)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();
        List<Vector2Int> frontier = new List<Vector2Int> { coord };

        for (int i = 0; i < radius; i++)
        {
            List<Vector2Int> nextFrontier = new List<Vector2Int>();
            foreach (Vector2Int current in frontier)
            {
                foreach (Vector2Int neighbor in GetNeighbors(current))
                {
                    if (!result.Contains(neighbor) && neighbor != coord)
                    {
                        result.Add(neighbor);
                        nextFrontier.Add(neighbor);
                    }
                }
            }
            frontier = nextFrontier;
        }

        return new List<Vector2Int>(result);
    }

    public Dictionary<Vector2Int,HexInfos> PathFindingBfs(Vector2Int start, Vector2Int end)
    {
        var path = new List<Vector2Int> {};

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        
        var neighbors = GetNeighbors(start);
        var cameFrom = new Dictionary<Vector2Int, Vector2Int> { [start] = start  };

        var pathFound = false;

        Vector2Int current = new Vector2Int();
        
        while (queue.Count > 0)
        {
            current = queue.Dequeue();

            if (current == end)
            {
                pathFound = true;
                break;
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!pathFound)
            return new Dictionary<Vector2Int,HexInfos>();

        current = end;
        while (current != start)
        {
            path.Add(current);
            current =  cameFrom[current];
        }
        
        path.Add(start);
        path.Reverse();
        
        Dictionary<Vector2Int,HexInfos> pathObjects = new Dictionary<Vector2Int, HexInfos>();

        foreach (Vector2Int coord in path)
            pathObjects.Add(coord,_hexes[coord]);
        
        return pathObjects;
    }

    public List<Vector2Int> GetNeighbors(Vector2Int coord, Dictionary<Vector2Int, HexInfos> newList)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] offsets = coord.y % 2 == 0 ? _evenOffsets : _oddOffsets;

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int offsetCoord = coord + offset;
            if (newList.ContainsKey(offsetCoord))
                neighbors.Add(offsetCoord);
        }
        return neighbors;
    }

    public Dictionary<Vector2Int, HexInfos> PathFindingBfs(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, HexInfos > newList)
    {
        var path = new List<Vector2Int> {};

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        var neighbors = GetNeighbors(start,newList);
        var cameFrom = new Dictionary<Vector2Int, Vector2Int> { [start] = start };

        var pathFound = false;

        Vector2Int current = new Vector2Int();

        while (queue.Count > 0)
        {
            current = queue.Dequeue();

            if (current == end)
            {
                pathFound = true;
                break;
            }

            foreach (Vector2Int neighbor in GetNeighbors(current, newList))
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!pathFound)
            return new Dictionary<Vector2Int, HexInfos>();

        current = end;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();

        Dictionary<Vector2Int, HexInfos> pathObjects = new Dictionary<Vector2Int, HexInfos>();

        foreach (Vector2Int coord in path)
            pathObjects.Add(coord, _hexes[coord]);

        return pathObjects;
    }

    public void OutlineHexes(Vector2Int startPos, int nbAction, bool value) 
    {
        /*foreach (Vector2Int pos in PathfindingBFS.Instance.GetNeighbors(PlayerMovement.instance.GetPosition(), nbAction))
        {
            PathfindingBFS.Instance._hexes[pos].AddOutline(value ? outlineMat : holderMat);
            PathfindingBFS.Instance._hexes[pos].walkable = value;
        }*/
    }
}
