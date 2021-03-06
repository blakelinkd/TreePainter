using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;
using System.Collections.Generic;
//using SimpleValidator;
//using SimpleValidator.Exceptions;
//using TinyJson;
using TinyJSON;
using System.Linq;

public class TreePainter : EditorWindow
{


    public static string configFilePath = "Assets/Editor/TreePainter/config.json";


    [SerializeField] Config configuration;
    PrefabCollection activeCollection;
    Label prevListElement;
    Label currListElement;
    GameObject activeTerrain;
    private VisualElement dragAndDropArea;
    public static bool config;
    public List<GameObject> activePrefabCollection = new List<GameObject>();
    //[MenuItem("Paint Trees/Setup")]

    class RightClick : MonoBehaviour
    {
        [MenuItem("GameObject/TreePainter", false, 0)]
        public static void TreePainter()
        {
            config = configExists(configFilePath);
            TreePainter wnd = GetWindow<TreePainter>();
            wnd.titleContent = new GUIContent("TreePainter");
            Vector2 windowSize = new Vector2(400, 600);
            wnd.minSize = windowSize;
        }
    }

    public void OnEnable()
    {
        int Counter = 0;
        Debug.Log($">>>>>>>>>{Counter += 3}>>>>>>>>>>>>");
        checkConfig();







    }
    void OnInspectorUpdate()
    {



        var objectField = rootVisualElement.Q<ObjectField>("terrainObject");
        objectField.objectType = typeof(GameObject);
        objectField.allowSceneObjects = true;


        if (activeCollection != null && objectField.value != null)
        {
            var objectLabel = rootVisualElement.Q<Label>("terrainObjectLabel");
            objectLabel.text = $"Active Terrain Object: { objectField.value.name }: ";
            activeTerrain = (GameObject)objectField.value;

            List<GameObject> prefabList = new List<GameObject>();
            //foreach (var obj in activeCollection.object_collection)
            foreach (var obj in activePrefabCollection)
            {
                Debug.Log($"{obj.name} is type: { obj.GetType() } ");

                prefabList.Add(obj);
            }




            TerrainWriter terrainWriter = new TerrainWriter(activeTerrain);

            terrainWriter.writeTerrain(prefabList, activeTerrain);
            PrefabList();


        }

        Repaint();
    }
    private void checkConfig()
    {
        if (!config)
        {
            createConfig();
        }
        configuration = GetConfig(configFilePath);



    }
    public void OnListSelect(MouseDownEvent e, string text)
    {
        if (e.clickCount > 1)
        {
            SetActiveCollection(text);
            PrefabList();

        }
    }

    public void SetActiveCollection(string name)
    {
        IEnumerable<PrefabCollection> collection = configuration.collections.Where(collection => collection.name == name);
        activeCollection = collection.First<PrefabCollection>();

        var activeCollectionLabel = rootVisualElement.Q<Label>("activeCollection");
        activeCollectionLabel.text = "active collection: ";

        var activeCollectionContent = rootVisualElement.Q<Label>("activeCollectionContent");
        activeCollectionContent.text = name;
        activeCollectionContent.ToggleInClassList("activeCollectionLabel");

        var listElement = rootVisualElement.Q<Label>(activeCollection.name);
        if (listElement != null)
        {
            listElement.ToggleInClassList("activeCollectionLabel");
            currListElement = listElement;

            if (prevListElement == null)
            {
                prevListElement = listElement;
            }
            else
            {
                prevListElement.ToggleInClassList("activeCollectionLabel");
                prevListElement = listElement;
            }


        }

    }

    public void addPrefabToActiveCollection(string newTree)
    {
        activeCollection.collection.Add(newTree);
    }

    private void DragAndDropArea()
    {
        dragAndDropArea = rootVisualElement.Q<Box>("RightCollectionsContainer");
        dragAndDropArea.RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
        dragAndDropArea.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
        dragAndDropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
    }


    private void OnDragUpdatedEvent(DragUpdatedEvent evt)
    {
        dragAndDropArea.AddToClassList("dragover");

        object draggedLabel = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
        if (draggedLabel != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        }
        else
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragLeaveEvent(DragLeaveEvent evt)
    {
        throw new NotImplementedException();
    }


    private void OnDragEnterEvent(DragEnterEvent evt)
    {
        DragAndDrop.AcceptDrag();

        foreach (var obj in DragAndDrop.objectReferences)
        {
            activeCollection.collection.Add(obj.name);
            activePrefabCollection.Add((GameObject)obj);
            activeCollection.object_collection.Add((GameObject)obj);
        }

        Config currentConfig = GetConfig(configFilePath);
        currentConfig.collections = configuration.collections;

        Debug.Log("dump: " + JSON.Dump(currentConfig));




        //Debug.Log("json is: " + currentConfig.ToJson());

        File.WriteAllText(configFilePath, JSON.Dump(currentConfig));
        //File.WriteAllText(configFilePath, currentConfig.ToJson());


        PrefabList();


    }



    private void clearPrefabListView()
    {
        var listView = rootVisualElement.Q<ListView>("RightListView");
        var listParent = listView.parent;

    }
    private void PrefabList()
    {

        if (activeCollection != null && activeCollection.collection.Count >= 0)
        {
            clearPrefabListView();
            var listView = rootVisualElement.Q<ListView>("RightListView");
            var listItems = new List<string>();
            for (int i = 0; i < activeCollection.collection.Count; ++i)
                listItems.Add(activeCollection.collection[i]);

            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                if (i >= activeCollection.collection.Count)
                    return;
                (e as Label).text = listItems[i];
                (e as Label).name = listItems[i];
            };

            listView.makeItem = makeItem;
            listView.bindItem = bindItem;
            listView.itemsSource = listItems;
            listView.selectionType = SelectionType.Multiple;
        }

    }


    private void CollectionList()
    {
        Debug.Log(configuration.collections.Count() + " collections exist");

        var listItems = new List<string>(configuration.collections.Count);
        for (int i = 0; i <= configuration.collections.Count - 1; i++)
            listItems.Add(configuration.collections[i].name);

        Func<VisualElement> makeItem = () => new Label();

        Action<VisualElement, int> bindItem = (e, i) => { (e as Label).text = listItems[i]; (e as Label).name = listItems[i]; e.RegisterCallback<MouseDownEvent>((_e) => OnListSelect(_e, listItems[i])); };

        var listView = rootVisualElement.Q<ListView>("listView");
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemsSource = listItems;
        listView.selectionType = SelectionType.Multiple;

        if (activeCollection != null)
        {
            SetActiveCollection(activeCollection.name);
        }

    }
    public void CreateGUI()
    {

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/TreePainter/USS/TreePainter.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        var HeaderUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TreePainter/UXML/Header.uxml");
        VisualElement elementsFromHeaderUXML = HeaderUXML.Instantiate();
        rootVisualElement.Add(elementsFromHeaderUXML);



        var CollectionListUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TreePainter/UXML/CollectionList.uxml");
        VisualElement elementsFromCollectionListUXML = CollectionListUXML.Instantiate();
        rootVisualElement.Add(elementsFromCollectionListUXML);


        var createCollectionML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TreePainter/UXML/CreateCollection.uxml");
        VisualElement elementsFromCreateCollectionML = createCollectionML.Instantiate();

        CollectionList();
        DragAndDropArea();

        Action<string> validateAndCreateCollection = (name) =>
        {
            createCollection(name);
            configuration = GetConfig(configFilePath);
            SetActiveCollection(name);
            CollectionList();

        };

        rootVisualElement.Add(elementsFromCreateCollectionML);
        var createButton = rootVisualElement.Q<Button>("createButton");
        var collectionNameField = rootVisualElement.Q<TextField>("newCollectionName");
        createButton.RegisterCallback<MouseUpEvent>((evt) => validateAndCreateCollection(collectionNameField.text));

        var footerUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TreePainter/UXML/Footer.uxml");
        VisualElement elementsFromFooterUXML = footerUXML.Instantiate();
        rootVisualElement.Add(elementsFromFooterUXML);
    }

    private static bool configExists(string configFilePath)
    {
        if (File.Exists(configFilePath))
        {
            return true;
        }

        return false;
    }


    private Config GetConfig(string name)
    {
        string fileJson = File.ReadAllText(configFilePath);
        //Debug.Log("Loaded config data: " + fileJson);
        //Config config = fileJson.FromJson<Config>();
        //Variant data = JSON.Load(fileJson);
        //var shit = JSON.Dump(fileJson, EncodeOptions.None);
        Config output = new Config();
        output = JSON.Load(fileJson).Make<Config>();

        //JSON.MakeInto(JSON.Load(shit), out output);

        return output;

    }

    private void createConfig()
    {
        Config newConfig = new Config();
        List<string> data = new List<string>();

        string poop = JSON.Dump(newConfig, EncodeOptions.PrettyPrint);



        if (!File.Exists(configFilePath))
        {

            File.WriteAllText(configFilePath, poop);

        }
    }
    private void createCollection(string name)
    {
        if (!config)
        {
            createConfig();

        }

        PrefabCollection newCollection = new PrefabCollection();

        IEnumerable<PrefabCollection> collectionNames = configuration.collections.Where(collection => collection.name == name);
        if (collectionNames.Count() > 0)
        {
            foreach (var _name in collectionNames)
            {
                if (_name.name.Contains('.'))
                {
                    var splitName = _name.name.Split('.');
                    Debug.Log("split: 0: " + splitName[0] + " split: 1: " + splitName[1]);
                    Debug.Log("splitName[1] to int: " + Int32.Parse(splitName[1]));


                    int newPostfix = Int32.Parse(splitName[1]) + 1;
                    string incPostfix = newPostfix.ToString().PadLeft(3, '0');
                    name = $"{splitName[0]}.{incPostfix}";
                }
                else
                {
                    name = name + ".001";
                }
            }
        }


        newCollection.name = name;



        Debug.Log("new collection name should be: " + name + ", it really is " + newCollection.name);




        Config currentConfig = GetConfig(configFilePath);
        currentConfig.collections.Add(newCollection);

        currentConfig.collections.Add(newCollection);


        var penis = JSON.Dump(currentConfig);
        Debug.Log("PENIS JSON : " + penis);

        //string jsonPaintCollection = data.ToString();

        File.WriteAllText(configFilePath, penis);




    }
}