using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PrefabCollection : MonoBehaviour
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