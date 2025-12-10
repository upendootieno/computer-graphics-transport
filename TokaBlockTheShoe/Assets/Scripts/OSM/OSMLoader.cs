using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class OSMLoader
{
    public class Node
    {
        public long Id;
        public double Lat;
        public double Lon;
    }

    public class Way
    {
        public long Id;
        public List<long> NodeIds = new List<long>();
    }

    public List<Node> Nodes = new List<Node>();
    public List<Way> Ways = new List<Way>();

    public void Load(string path)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(path);

        XmlNodeList nodeList = xml.SelectNodes("//node");
        foreach (XmlNode node in nodeList)
        {
            long id = long.Parse(node.Attributes["id"].Value);
            double lat = double.Parse(node.Attributes["lat"].Value, System.Globalization.CultureInfo.InvariantCulture);
            double lon = double.Parse(node.Attributes["lon"].Value, System.Globalization.CultureInfo.InvariantCulture);

            Nodes.Add(new Node { Id = id, Lat = lat, Lon = lon });
        }

        XmlNodeList wayList = xml.SelectNodes("//way");
        foreach (XmlNode way in wayList)
        {
            Way w = new Way();
            w.Id = long.Parse(way.Attributes["id"].Value);

            XmlNodeList ndList = way.SelectNodes("./nd");
            foreach (XmlNode nd in ndList)
            {
                long refId = long.Parse(nd.Attributes["ref"].Value);
                w.NodeIds.Add(refId);
            }

            Ways.Add(w);
        }

        Debug.Log("OSM Loaded: " + Nodes.Count + " nodes, " + Ways.Count + " ways");
    }
}
