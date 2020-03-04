using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class NodeBasedEditor : EditorWindow
{
    private List<Node> nodes;
    private List<Connection> connections;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    private Vector2 scrollPos;
    private Vector2 windowSize;

    // File locations
    string skillPath = null;
    string nodePath = null;

    // Textures
    private Texture2D panelTexture;

    // Rect for buttons to Clear, Save and Load 
    private Rect rectButtonClear;
    private Rect rectButtonSave;
    private Rect rectButtonLoad;

    // Rect for main area
    private Rect mainArea;

    // Count for nodes created
    private int nodeCount;

    // Current skill
    private Skill selected;

    // Where we store the skilltree that we are managing with this tool
    private SkillTree skillTree;

    // Dictionary with the skills in our skilltree
    private Dictionary<int, Skill> skillDictionary;

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        // Create the skilltree
        skillTree = new SkillTree();
        selected = null;

        // Create rect for main area
        mainArea = new Rect(0, 0, position.width - 215, position.height);

        // Create buttons for clear, save and load
        rectButtonClear = new Rect(10, 10, 60, 20);
        rectButtonSave = new Rect(new Vector2(80, 10), new Vector2(60, 20));
        rectButtonLoad = new Rect(new Vector2(150, 10), new Vector2(60, 20));

        // Initialize nodes with saved data
        //LoadNodes();
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        // We draw our new buttons (Clear, Load and Save)
        DrawButtons();
        DrawVLine();
        DrawPanel();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed)
            Repaint();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    // Draw our new buttons for managing the skill tree
    private void DrawButtons()
    {
        GUILayout.BeginArea(new Rect(0, 0, position.width - 215, 20), EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(new GUIContent("Clear"), EditorStyles.toolbarButton))
            ClearNodes();

        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton)) {
            string skillDir = "Assets/SkillTree/Data/";
            string skillFile = "";
            if (skillPath.Length != 0)
            {
                skillDir = System.IO.Path.GetDirectoryName(skillPath);
                skillFile = System.IO.Path.GetFileNameWithoutExtension(skillPath);
            }
            string temp = EditorUtility.SaveFilePanel("Save Skills", skillDir, skillFile, "json");
            if (temp.Length != 0)
            {
                skillPath = temp;
                string nodeDir = System.IO.Path.GetDirectoryName(skillPath);
                string nodeFile = "NodeData_" + System.IO.Path.GetFileNameWithoutExtension(skillPath) + ".json";
                nodePath = System.IO.Path.Combine(nodeDir, nodeFile);
                SaveSkillTree();
            }
        }

        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Load"), EditorStyles.toolbarButton))
        {
            skillPath = EditorUtility.OpenFilePanel("Load Skills", "Assets/SkillTree/Data/", "json");
            if (skillPath.Length != 0)
            {
                string nodeDirectory = System.IO.Path.GetDirectoryName(skillPath);
                string nodeFile = "NodeData_" + System.IO.Path.GetFileNameWithoutExtension(skillPath) + ".json";
                nodePath = System.IO.Path.Combine(nodeDirectory, nodeFile);
                LoadNodes();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    // Draw a vertical line
    private void DrawVLine()
    {
        GUIStyle style = new GUIStyle();
        style.normal.background = GetVerticalTexture();
        Rect line = new Rect(position.width - 218, 0, 3, position.height);
        GUILayout.BeginArea(line, style);
        GUILayout.EndArea();
    }

    // Draw side panel
    private void DrawPanel()
    {
        EditorGUIUtility.labelWidth = 86;
        EditorStyles.textField.wordWrap = true;
        GUIStyle style = new GUIStyle();
        style.normal.background = GetPanelTexture();
        Rect panelRect = new Rect(position.width - 215, 0, 215, position.height);
        GUILayout.BeginArea(panelRect, style);

        // Title
        EditorGUILayout.LabelField("Skill Editor", EditorStyles.toolbarButton);
        GUILayout.Space(5);

        if (selected == null)
        {
            GUILayout.EndArea();
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Name Field
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Name: ", "Name of the skill."));
        selected.name = EditorGUILayout.TextField(selected.name, GUILayout.Width(108));
        GUILayout.EndHorizontal();

        // Level Field
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Level: ", "Participant level at which this skill becomes available."));
        selected.level_req = EditorGUILayout.IntField(selected.level_req, GUILayout.Width(108));
        GUILayout.EndHorizontal();

        // Unlocked Field
        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Unlocked: ", "Specifies whether the particpant has unlocked this skill."));
        selected.unlocked = EditorGUILayout.Toggle(selected.unlocked, GUILayout.Width(108));
        GUILayout.EndHorizontal();

        // Description Area
        EditorGUILayout.PrefixLabel(new GUIContent("Description: ", "Description of the skill to be displayed to the participant."));
        selected.description = EditorGUILayout.TextArea(selected.description, GUILayout.Height(40));
        GUILayout.Space(10);
        DrawUILine();

        //----------------------------\\
        //          EFFECTS           \\
        //----------------------------\\
        List<BattleEffect> remove = new List<BattleEffect>();

        // List Effects
        foreach (BattleEffect effect in selected.effects)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle("ToolbarButton"));
            effect.Show = EditorGUILayout.Foldout(effect.Show, "Effect (" + effect.Type + ")");
            GUIContent popupIcon = EditorGUIUtility.IconContent("winbtn_win_close");
            if (GUILayout.Button(popupIcon, new GUIStyle(), GUILayout.Width(20), GUILayout.Height(20)))
            {
                remove.Add(effect);
            }
            EditorGUILayout.EndHorizontal();

            if (effect.Show)
            {
                // Effect Type Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Type: ", "???"));
                effect.Type = (EffectType)EditorGUILayout.EnumPopup(effect.Type, GUILayout.Width(108));
                GUILayout.EndHorizontal();

                // Modifier Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Modifier: ", "Specifies how strong the effect should be."));
                effect.Modifier = EditorGUILayout.FloatField(effect.Modifier, GUILayout.Width(108));
                GUILayout.EndHorizontal();

                // Duration Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Duration: ", "How many turns this effect lasts."));
                effect.Duration = EditorGUILayout.FloatField(effect.Duration, GUILayout.Width(108));
                GUILayout.EndHorizontal();

                // Message Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Specifier: ", "???"));
                effect.Specifier = EditorGUILayout.TextField(effect.Specifier, GUILayout.Width(108));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            DrawUILine();
        }

        // Remove effects
        foreach (BattleEffect effect in remove)
        {
            selected.effects.Remove(effect);
        }

        //----------------------------\\
        //          PASSIVES          \\
        //----------------------------\\
        remove = new List<BattleEffect>();

        // List Effects
        foreach (BattleEffect effect in selected.passives)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle("ToolbarButton"));
            effect.Show = EditorGUILayout.Foldout(effect.Show, "Passive");
            GUIContent popupIcon = EditorGUIUtility.IconContent("winbtn_win_close");
            if (GUILayout.Button(popupIcon, new GUIStyle(), GUILayout.Width(20), GUILayout.Height(20)))
            {
                remove.Add(effect);
            }
            EditorGUILayout.EndHorizontal();

            if (effect.Show)
            {
                // Effect Type Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Type: ", "???"));
                effect.Type = (EffectType)EditorGUILayout.EnumPopup(effect.Type, GUILayout.Width(108));
                GUILayout.EndHorizontal();

                // Modifier Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Modifier: ", "Specifies how strong the effect should be."));
                effect.Modifier = EditorGUILayout.FloatField(effect.Modifier, GUILayout.Width(108));
                GUILayout.EndHorizontal();

                // Message Field
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Specifier: ", "???"));
                effect.Specifier = EditorGUILayout.TextField(effect.Specifier, GUILayout.Width(108));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            DrawUILine();
        }

        // Remove passive
        foreach (BattleEffect effect in remove)
        {
            selected.passives.Remove(effect);
        }



        // Add effect
        GUILayout.Space(5);
        if (GUILayout.Button("Add Effect"))
        {
            selected.effects.Add(new BattleEffect());
        }

        // Add passive
        GUILayout.Space(5);
        if (GUILayout.Button("Add Passive"))
        {
            selected.passives.Add(new BattleEffect());
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    // Get panel background
    private Texture2D GetPanelTexture()
    {
        if (windowSize != position.size)
        {
            int width = (int)position.width - 215;
            int height = (int)position.height;
            Color col = new Color32(194, 194, 194, 255);
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            panelTexture = result;
            windowSize = position.size;
        }
       
        return panelTexture;
    }

    // Get vertical line
    private Texture2D GetVerticalTexture()
    {
        int width = 3;
        int height = (int)position.height;
        Color col = new Color32(138, 138, 138, 255);
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    // Draw a horizontal line
    public static void DrawUILine()
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(-2));
        r.height = 1;
        r.y += -2;
        r.x -= 3;
        r.width += 6;
        EditorGUI.DrawRect(r, new Color32(127, 127, 127, 255));
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        if (mainArea.Contains(e.mousePosition))
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    GUI.FocusControl(null);
                    if (e.button == 0)
                    {
                        ClearConnectionSelection();
                    }

                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }

            if (e.type == EventType.MouseDown)
            {
                bool flag = false;
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    if (!flag && nodes[i].rect.Contains(e.mousePosition))
                    {
                        selected = nodes[i].skill;
                        nodes[i].isSelected = true;
                        nodes[i].style = nodes[i].selectedNodeStyle;
                        flag = true;
                    }
                    else
                    {
                        nodes[i].isSelected = true;
                        nodes[i].style = nodes[i].defaultNodeStyle;
                    }
                }
            }
            
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        // We create the node with the default info for the node
        nodes.Add(new Node(mousePosition, 175, 50, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, new Skill(nodeCount)));
        nodeCount++;
    }

    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(Node node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }
    
    // Function for clearing data from the editor window
    private void ClearNodes()
    {
        nodeCount = 0;
        if (nodes != null && nodes.Count > 0)
        {
            Node node;
            while (nodes.Count > 0)
            {
                node = nodes[0];

                OnClickRemoveNode(node);
            }
        }
    }
    
    // Save data from the window to the skill tree
    private void SaveSkillTree()
    {
        if (nodes.Count > 0)
        {
            // We fill with as many skills as nodes we have
            skillTree.skilltree = new Skill[nodes.Count];
            int dependency = -1;

            // Iterate over all of the nodes. Populating the skills with the node info
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (connections != null)
                {
                    List<Connection> connectionsToRemove = new List<Connection>();
                    List<ConnectionPoint> connectionsPointsToCheck = new List<ConnectionPoint>();

                    for (int j = 0; j < connections.Count; j++)
                    {
                        if (connections[j].inPoint == nodes[i].inPoint)
                        {
                            for (int k = 0; k < nodes.Count; ++k)
                            {
                                if (connections[j].outPoint == nodes[k].outPoint)
                                {
                                    dependency = nodes[k].skill.id_Skill;
                                    break;
                                }
                            }
                            connectionsToRemove.Add(connections[j]);
                            connectionsPointsToCheck.Add(connections[j].outPoint);
                        }
                    }
                }
                skillTree.skilltree[i] = nodes[i].skill;
                skillTree.skilltree[i].pre_req = dependency;
            }

            string json = JsonUtility.ToJson(skillTree);

            //path = "Assets/SkillTree/Data/skilltree.json";

            // Finally, we write the JSON string with the SkillTree data in our file
            using (FileStream fs = new FileStream(skillPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                }
            }
            UnityEditor.AssetDatabase.Refresh();

            SaveNodes();
        }
    }

    // Save data from the nodes (position in our custom editor window)
    private void SaveNodes()
    {
        NodeDataCollection nodeData = new NodeDataCollection();
        nodeData.nodeDataCollection = new NodeData[nodes.Count];

        for (int i = 0; i < nodes.Count; ++i)
        {
            nodeData.nodeDataCollection[i] = new NodeData();
            nodeData.nodeDataCollection[i].id_Node = nodes[i].skill.id_Skill;
            nodeData.nodeDataCollection[i].position = nodes[i].rect.position;
        }

        string json = JsonUtility.ToJson(nodeData);
        //string path = "Assets/SkillTree/Data/nodeData.json";

        using (FileStream fs = new FileStream(nodePath, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }
        UnityEditor.AssetDatabase.Refresh();
    }
    
    private void LoadNodes()
    {
        ClearNodes();

        //string path = "Assets/SkillTree/Data/nodeData.json";
        string dataAsJson;
        NodeDataCollection loadedData;
        if (File.Exists(nodePath))
        {
            // Read the json from the file into a string
            dataAsJson = File.ReadAllText(nodePath);

            // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
            loadedData = JsonUtility.FromJson<NodeDataCollection>(dataAsJson);

            Skill[] _skillTree;
            List<Skill> originNode = new List<Skill>();
            skillDictionary = new Dictionary<int, Skill>();
            //path = "Assets/SkillTree/Data/skilltree.json";
            Vector2 pos = Vector2.zero;
            if (File.Exists(skillPath))
            {
                // Read the json from the file into a string
                dataAsJson = File.ReadAllText(skillPath);

                // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
                SkillTree skillData = JsonUtility.FromJson<SkillTree>(dataAsJson);

                // Store the SkillTree as an array of Skill
                _skillTree = new Skill[skillData.skilltree.Length];
                _skillTree = skillData.skilltree;

                // Create nodes
                for (int i = 0; i < _skillTree.Length; ++i)
                {
                    for (int j = 0; j < loadedData.nodeDataCollection.Length; ++j)
                    {
                        if (loadedData.nodeDataCollection[j].id_Node == _skillTree[i].id_Skill)
                        {
                            pos = loadedData.nodeDataCollection[j].position;
                            break;
                        }
                    }
                    LoadSkillCreateNode(_skillTree[i], pos);
                    if (_skillTree[i].pre_req == -1)
                    {
                        originNode.Add(_skillTree[i]);
                    }
                    skillDictionary.Add(_skillTree[i].id_Skill, _skillTree[i]);
                }

                Skill outSkill;
                Node outNode = null;
                // Create connections
                for (int i = 0; i < nodes.Count; ++i)
                {
                        if (skillDictionary.TryGetValue(nodes[i].skill.pre_req, out outSkill))
                        {
                            for (int k = 0; k < nodes.Count; ++k)
                            {
                                if (nodes[k].skill.id_Skill == outSkill.id_Skill)
                                {
                                    outNode = nodes[k];
                                    OnClickOutPoint(outNode.outPoint);
                                    break;
                                }
                            }
                            OnClickInPoint(nodes[i].inPoint);
                        }
                }
            }
            else
            {
                Debug.LogError("Cannot load game data!");
            }
        }
        selected = nodes[0].skill;
    }

    private void LoadSkillCreateNode(Skill skill, Vector2 position)
    {
        if (nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new Node(position, 175, 50, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, skill));
        nodeCount = Mathf.Max(nodeCount, skill.id_Skill) + 1;
    }
}