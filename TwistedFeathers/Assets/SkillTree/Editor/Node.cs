using System;
using UnityEditor;
using UnityEngine;
using System.Text;

public class Node
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    // Rect for the name
    public Rect rectNameLabel;
    public Rect rectName;

    // Rect for the id of the node 
    public Rect rectID;

    // Two Rect for the description field (1 for the label and other for the text field)
    public Rect rectDescLabel;
    public Rect rectDesc;

    // Two Rect for the unlock field (1 for the label and other for the checkbox)
    public Rect rectUnlockLabel;
    public Rect rectUnlocked;

    // Two Rect for the level field (1 for the label and other for the text field)
    public Rect rectLevelLabel;
    public Rect rectLevel;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    // GUI Style for the id
    public GUIStyle styleID;

    // GUI Style for the fields
    public GUIStyle styleField;

    public Action<Node> OnRemoveNode;

    // Skill linked with the node
    public Skill skill;

    // Bool for checking if the node is unlocked or not
    private bool unlocked = false;

    // StringBuilder to create the node's title
    private StringBuilder nodeTitle;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, 
        GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, 
        Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint,
        Action<Node> OnClickRemoveNode, int id, string name, string desc, bool unlocked, int level_req, int dependency)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;

        // Create new Rect and GUIStyle for our title and custom fields
        float rowHeight = height / 7;
        float adj_x = position.x + 10;
        float adj_width = (width - 20) / 5;

        styleField = new GUIStyle();
        styleField.alignment = TextAnchor.UpperRight;


        rectID = new Rect(position.x, position.y + 2 * rowHeight, width, rowHeight);
        styleID = new GUIStyle();
        styleID.alignment = TextAnchor.UpperCenter;

        rectNameLabel = new Rect(adj_x, position.y + rowHeight, adj_width * 2, rowHeight);
        rectName = new Rect(adj_x + adj_width * 2, position.y + rowHeight, adj_width * 3, rowHeight);

        rectLevelLabel = new Rect(adj_x, position.y + rowHeight * 2, adj_width * 2, rowHeight);
        rectLevel = new Rect(adj_x + adj_width * 2, position.y + rowHeight * 2, adj_width * 3, rowHeight);

        rectUnlockLabel = new Rect(adj_x, position.y + rowHeight * 3, adj_width * 2, rowHeight);
        rectUnlocked = new Rect(adj_x + adj_width * 2, position.y + rowHeight * 3, adj_width * 3, rowHeight);

        rectDescLabel = new Rect(adj_x, position.y + rowHeight * 4, adj_width * 2, rowHeight);
        rectDesc = new Rect(adj_x, position.y + rowHeight * 5, adj_width * 5, rowHeight);

        this.unlocked = unlocked;

        // We create the skill with current node info
        skill = new Skill();
        skill.name = name;
        skill.id_Skill = id;
        skill.description = desc;
        skill.unlocked = unlocked;
        skill.level_req = level_req;
        skill.pre_req = dependency;

        // Create string with ID info
        nodeTitle = new StringBuilder();
        nodeTitle.Append("ID: ");
        nodeTitle.Append(id);

    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
        rectNameLabel.position += delta;
        rectName.position += delta;
        rectID.position += delta;
        rectUnlocked.position += delta;
        rectUnlockLabel.position += delta;
        rectLevel.position += delta;
        rectLevelLabel.position += delta;
        rectDescLabel.position += delta;
        rectDesc.position += delta;
    }

    public void MoveTo(Vector2 pos)
    {
        rect.position = pos;
        rectNameLabel.position = pos;
        rectName.position = pos;
        rectID.position = pos;
        rectUnlocked.position = pos;
        rectUnlockLabel.position = pos;
        rectLevel.position = pos;
        rectLevelLabel.position = pos;
        rectDescLabel.position = pos;
        rectDesc.position = pos;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);

        // Print the name
        GUIStyle styleCenter = new GUIStyle(GUI.skin.box);
        styleCenter.alignment = TextAnchor.MiddleCenter;

        GUI.Label(rectNameLabel, "Name: ");
        skill.name = GUI.TextField(rectName, skill.name);

        // Print the id
        //GUI.Label(rectID, nodeTitle.ToString(), styleID);

        // Print the level field
        GUI.Label(rectLevelLabel, "Level: ");
        skill.level_req = int.Parse(GUI.TextField(rectLevel, skill.level_req.ToString()));

        // Print the unlock field
        GUI.Label(rectUnlockLabel, "Unlocked: ");
        if (GUI.Toggle(rectUnlocked, unlocked, ""))
            unlocked = true;
        else
            unlocked = false;

        skill.unlocked = unlocked;

        // Print the description area
        GUI.Label(rectDescLabel, "Description:");
        skill.description = GUI.TextArea(rectDesc, skill.description);
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}