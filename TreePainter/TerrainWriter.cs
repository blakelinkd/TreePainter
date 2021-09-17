using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UI;


public class TerrainWriter : MonoBehaviour
{

    GameObject _terrainAsset;
    public TerrainData _terrainData;
    List<TreePrototype> treePrototypes = new List<TreePrototype>();

    public TerrainWriter(GameObject terrain)
    {
        _terrainAsset = terrain;
    }

    public void writeTerrain(List<GameObject> prefabs, GameObject terrain)
    {
        //GameObject terrainObject = GameObject.Find(_terrainAsset.name);
        GameObject terrainObject = terrain;
        _terrainData = terrainObject.GetComponent<TerrainCollider>().terrainData;

        foreach (var prefab in prefabs)
        {



            //var prefabFromString = GameObject.Find(prefab);


            var treePrefab = new TreePrototype();
            treePrefab.prefab = prefab;

            // string localPath = "Assets/" + gameObject.name + ".prefab";
            // localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            // var newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);


            //Instantiate(prefab);
            treePrototypes.Add(treePrefab);

        }

        foreach (var item in treePrototypes)
        {
            Debug.Log($"prefab name: {item.prefab.name}");
        }

        _terrainData.treePrototypes = treePrototypes.ToArray();
    }

}