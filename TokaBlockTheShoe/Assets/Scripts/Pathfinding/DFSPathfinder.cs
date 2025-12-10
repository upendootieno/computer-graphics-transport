using System.Collections.Generic;

public class DFSPathfinder
{
    public List<long> FindPath(Dictionary<long, List<long>> graph, long start, long goal)
    {
        var visited = new HashSet<long>();
        var path = new List<long>();

        bool found = DFS(start, goal, graph, visited, path);

        return found ? path : null;
    }

    private bool DFS(long current, long goal, Dictionary<long, List<long>> graph, HashSet<long> visited, List<long> path)
    {
        visited.Add(current);
        path.Add(current);

        if(current == goal)
            return true;

        foreach(var next in graph[current])
        {
            if(!visited.Contains(next))
            {
                if(DFS(next, goal, graph, visited, path))
                    return true;
            }
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }
}
