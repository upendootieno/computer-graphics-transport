using System.Collections.Generic;

public class GraphBuilder
{
    public Dictionary<long, List<long>> Adjacency = new Dictionary<long, List<long>>();

    public void Build(OSMLoader loader)
    {
        // init list
        foreach(var node in loader.Nodes)
            Adjacency[node.Id] = new List<long>();

        // create edges
        foreach(var way in loader.Ways)
        {
            for(int i = 0; i < way.NodeIds.Count - 1; i++)
            {
                long a = way.NodeIds[i];
                long b = way.NodeIds[i + 1];

                Adjacency[a].Add(b);
                Adjacency[b].Add(a); // treat as bi-directional road
            }
        }
    }
}
