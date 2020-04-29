using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TwistedFeathers;

public class UpgradeManager : MonoBehaviour
{

    //player that is upgrading their skils
    public Player upgrader;

    //Skills to pick
    List<Skill> pickUs;

    //is Player in upgrade phase?
    public bool done;
    //buttons for upgrade options
    public Button u1,u2,u3,u4;
    //reroll button
    public Button Re;
    private bool rerolled = false;

    //Strings of Skill options on buttons
    public string opt1, opt2, opt3, opt4;
    //Strings of enemy skill options on buttons
    public string opte1, opte2, opte3, opte4, optReE;
    public string constRe = "Reroll";
    public string dash = " - ";
    //needs to:
    //  get the player's current skills
    //  get the pool of available skills
    //  decide skills to offer based off of criteria

    private void Start()
    {
        upgrader = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player1;
        done = false;
        u1.name = "u1";
        u2.name = "u2";
        u3.name = "u3";
        u4.name = "u4";
        populate();
    }

    void FixedUpdate()
    {

        
    }

    public void populate()
    {
        //retrieve skill pool
        //fetch 4 skills from pool
        //  add criteria later
        //parse names, combine them with enemy skills
        //set names to text of buttons b1-b4
        Dictionary<string, Skill> skillPool = GameManager.Skill_db;
        //skillNames = skillPool.Keys;
        //int size = skillNames.count;
        List<Skill> foundPlayerSkills = new List<Skill>();
        List<Skill> foundEnemySkills = new List<Skill>();
        pickUs = Enumerable.ToList(skillPool.Values);

        while (foundPlayerSkills.Count < 4)
        {
            int i = (int)Random.Range(0, skillPool.Count - 1);
            //XXX = ; //Random between 0-size
            if (pickUs[i].User_type == p_type.player)
            {
                if (!foundPlayerSkills.Contains(pickUs[i]))
                {
                    foundPlayerSkills.Add(pickUs[i]);
                }
            }
        }
        while(foundEnemySkills.Count < 5)
        {
            int i = (int)Random.Range(0, skillPool.Count - 1);
            //XXX = ; //Random between 0-size
            if (pickUs[i].User_type == p_type.enemy)
            {
                if (!foundEnemySkills.Contains(pickUs[i]))
                {
                    foundEnemySkills.Add(pickUs[i]);
                }
            }
        }
        opt1 = foundPlayerSkills[0].Name;
        opt2 = foundPlayerSkills[1].Name;
        opt3 = foundPlayerSkills[2].Name;
        opt4 = foundPlayerSkills[3].Name;
        opte1 = foundEnemySkills[0].Name;
        opte2 = foundEnemySkills[1].Name;
        opte3 = foundEnemySkills[2].Name;
        opte4 = foundEnemySkills[3].Name;
        optReE = foundEnemySkills[4].Name;

        //Use strings
        u1.transform.GetChild(0).GetComponentInChildren<Text>().text = opt1;
        u1.transform.GetChild(1).GetComponentInChildren<Text>().text = opte1;
        u2.transform.GetChild(0).GetComponentInChildren<Text>().text = opt2;
        u2.transform.GetChild(1).GetComponentInChildren<Text>().text = opte2;
        u3.transform.GetChild(0).GetComponentInChildren<Text>().text = opt3;
        u3.transform.GetChild(1).GetComponentInChildren<Text>().text = opte3;
        u4.transform.GetChild(0).GetComponentInChildren<Text>().text = opt4;
        u4.transform.GetChild(1).GetComponentInChildren<Text>().text = opte4;
        Re.transform.GetChild(0).GetComponentInChildren<Text>().text = constRe;
        Re.transform.GetChild(1).GetComponentInChildren<Text>().text = optReE;
    }

    /**Gets the text of the button, which has been set to the skill names.
     * It then splits the string to get the player skill, and searches the pickUs
     * list for a skill with that name. The function then calls the player learnSkill
     * function to complete the learning process.
     * @param name name of the button
     *
     */
    public void addSkill(GameObject name)
    { 
        Debug.Log("Attempting to Add Skill");
        string skillName;
        string[] splitter = new string[] { dash };
        //learn skill based off name
        
        if (!done)
        {
            skillName = name.GetComponent<Text>().text;
            Debug.Log("Skill upgrader is looking for: " + skillName);
            for(int i = 0; i < pickUs.Count; i++)
            {
                if(pickUs[i].Name == skillName)
                {
                    Debug.Log("Skill found "+ pickUs[i].Name);
                    upgrader.AddSkill(pickUs[i]);
                    break;
                }
            }
            //Skill picked = pickUs.
            done = true;
            Debug.Log("Should have chosen skill by now");
            Invoke("FinishUpgrade", 1f);

        }
    }


    public void Reroll()
    {
        //GameManager.eLearnedSkills.Add();
        if (!rerolled)
        {
            rerolled = true;
            populate();
        }
        //redo all skill names
        //add skill to enemy skill list
        //make reroll unavailable after clicking

    }

    private void FinishUpgrade()
    {
        SceneManager.LoadScene("TestScene");
        GameObject.Find("GameManager").GetComponent<GameManager>().comingBack();
    }

    public void goBack(){
        SceneManager.LoadScene("TestScene");
        GameObject.Find("GameManager").GetComponent<GameManager>().comingBack();
    }
}
