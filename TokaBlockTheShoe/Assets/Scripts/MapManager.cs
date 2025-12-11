using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public string OsmFilePath = "Assets/OSM/the_shoe_planet.osm";  // change to your file
    public VehicleController Vehicle;

    OSMLoader loader;
    GraphBuilder graph;
    DFSPathfinder dfs;

    void Start()
    {
        // load OSM
        loader = new OSMLoader();
        loader.Load(OsmFilePath);

        // build graph
        graph = new GraphBuilder();
        graph.Build(loader);

        // pick start and goal nodes arbitrarily
        long startNode = loader.Nodes[0].Id;
        long goalNode  = loader.Nodes[loader.Nodes.Count - 1].Id;

        // run DFS
        dfs = new DFSPathfinder();
        List<long> nodePath = dfs.FindPath(graph.Adjacency, startNode, goalNode);

        // convert node IDs to Unity positions
        List<Vector3> unityPath = new List<Vector3>();
        foreach(var id in nodePath)
        {
            var n = loader.Nodes.First(x => x.Id == id);
            unityPath.Add(LatLonToWorld(n.Lat, n.Lon));
        }

        Vehicle.SetFullPath(unityPath);

    }

    Vector3 LatLonToWorld(double lat, double lon)
    {
        float scale = 1000f;
        return new Vector3((float)(lon * scale), 0, (float)(lat * scale));
    }
}
