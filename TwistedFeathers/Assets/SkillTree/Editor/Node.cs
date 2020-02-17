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

    // Two Rect for the effect field (1 for the label and other for the checkbox)
    public Rect rectEffectLabel;
    public Rect rectEffect;

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
        Action<Node> OnClickRemoveNode, int id, string name, string desc, Effect effect, bool unlocked, int level_req, int dependency)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;

        // Create new Rect and GUIStyle for our title and custom fields
        float rowHeight = 18;
        float col_2 = 2 * (width - 20) / 5;
        float col_3 = 3 * (width - 20) / 5;
        float col_5 = (width - 20);
        float x_pos = position.x + 10;
        float y_pos = position.y + 10;
        float offset = 20;

        styleField = new GUIStyle();
        styleField.alignment = TextAnchor.UpperRight;


        //rectID = new Rect(position.x, position.y + 2 * rowHeight, width, rowHeight);
        //styleID = new GUIStyle();
        //styleID.alignment = TextAnchor.UpperCenter;

        rectNameLabel = new Rect(x_pos, y_pos, col_2, rowHeight);
        rectName = new Rect(x_pos + col_2, y_pos, col_3, rowHeight);

        rectLevelLabel = new Rect(x_pos, y_pos + offset, col_2, rowHeight);
        rectLevel = new Rect(x_pos + col_2, y_pos + offset, col_3, rowHeight);

        rectUnlockLabel = new Rect(x_pos, y_pos + offset * 2, col_2, rowHeight);
        rectUnlocked = new Rect(x_pos + col_2, y_pos + offset * 2, col_3, rowHeight);

        rectEffectLabel = new Rect(x_pos, y_pos + offset * 3, col_2, rowHeight);
        rectEffect = new Rect(x_pos + col_2, y_pos + offset * 3, col_3, rowHeight);

        rectDescLabel = new Rect(x_pos, y_pos + offset * 4, col_2, rowHeight);
        rectDesc = new Rect(x_pos, y_pos + offset * 5, col_5, rowHeight * 2);

        this.unlocked = unlocked;

        // We create the skill with current node info
        skill = new Skill();
        skill.name = name;
        skill.id_Skill = id;
        skill.description = desc;
        skill.effect = effect;
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
        rectEffect.position += delta;
        rectEffectLabel.position += delta;
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
        rectEffect.position = pos;
        rectEffectLabel.position = pos;
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
        EditorGUI.LabelField(rectNameLabel, "Name: ");
        skill.name = EditorGUI.TextField(rectName, skill.name);

        // Print the id
        //GUI.Label(rectID, nodeTitle.ToString(), styleID);

        // Print the level field
        EditorGUI.LabelField(rectLevelLabel, "Level: ");
        skill.level_req = EditorGUI.IntField(rectLevel, skill.level_req);

        // Print the unlock field
        EditorGUI.LabelField(rectUnlockLabel, "Unlocked: ");
        if (EditorGUI.Toggle(rectUnlocked, unlocked))
            unlocked = true;
        else
            unlocked = false;

        skill.unlocked = unlocked;

        // Print effect dropdown
        EditorGUI.LabelField(rectEffectLabel, "Effect: ");
        skill.effect = (Effect)EditorGUI.EnumPopup(rectEffect, skill.effect);

        // Print the description area
        EditorGUI.LabelField(rectDescLabel, "Description:");
        skill.description = EditorGUI.TextArea(rectDesc, skill.description);
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