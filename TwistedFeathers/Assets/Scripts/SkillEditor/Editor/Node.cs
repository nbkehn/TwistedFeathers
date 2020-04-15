using System;
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using TwistedFeathers;

public class Node
{
    public Rect rect;
    public bool isDragged;
    public bool isSelected;

    // Rect for the name
    public Rect rectName;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    public Action<Node> OnRemoveNode;

    // Skill linked with the node
    public Skill skill;

    public Node(Vector2 position, float width, float height, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint,
        Action<Node> OnClickRemoveNode, Skill newSkill)
    {
        rect = new Rect(position.x, position.y, width, height);
        skill = newSkill;

        // Define Styles //
        defaultNodeStyle = new GUIStyle();
        defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        style = defaultNodeStyle;
        // End Styles //

        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        OnRemoveNode = OnClickRemoveNode;

        // Create new Rect and GUIStyle for our title and custom fields
        float rowHeight = 18;
        float col_5 = (width - 20);
        float x_pos = position.x + 10;
        float y_pos = position.y + 13;

        rectName = new Rect(x_pos, y_pos, col_5, rowHeight);
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
        rectName.position += delta;
    }

    public void MoveTo(Vector2 pos)
    {
        rect.position = pos;
        rectName.position = pos;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, "", style);

        // Print the name
        EditorGUI.LabelField(rectName, skill.Name, EditorStyles.toolbarButton);
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
                        //isSelected = true;
                        //style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        //isSelected = false;
                        //style = defaultNodeStyle;
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