using System.Collections.Generic;
using UnityEngine;
public class PrefabCollection
{
    public string name { get; set; }
    public List<string> collection;
    public List<GameObject> object_collection;

    public PrefabCollection()
    {

        collection = new List<string>();
        object_collection = new List<GameObject>();
    }

}