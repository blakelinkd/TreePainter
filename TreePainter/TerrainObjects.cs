using System.Collections.Generic;
public class TerrainObjects
{

    public string name { get; set; }
    public List<PrefabCollection> collection;

    public TerrainObjects()
    {
        collection = new List<PrefabCollection>();
    }
}