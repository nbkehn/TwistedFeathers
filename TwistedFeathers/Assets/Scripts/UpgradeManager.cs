using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    //player that is upgrading their skils
    public Player upgrader;

    //Is player in Upgrade phase?
    public bool upPhase;
    //buttons for upgrade options
    public Button u1;
    public Button u2;
    public Button u3;
    public Button u4;
    //reroll button
    public Button Re;

    //Strings of Skill options on buttons
    public string opt1, opt2, opt3, opt4, optRe;
    public string constRe = "Reroll - ";
    //needs to:
    //  get the player's current skills
    //  get the pool of available skills
    //  decide skills to offer based off of criteria

    private void Start()
    {
        upPhase = true;
    }

    void Update()
    {
        if (upPhase)
        {
            //other.GetComponent<Scoop>().Scoopee = ingred;
            string button = Re.GetComponentInChildren<Text>().text;
            Re.GetComponentInChildren<Text>().text = constRe;
            Debug.Log(button);
        }
        else
        {
            //string button = Re.GetComponentInChildren<Text>().text;
            //Re.GetComponentInChildren<Text>().text = "Inactive";
            //Debug.Log(button);
        }
        //upgrader.skill_db.;
    }
}
