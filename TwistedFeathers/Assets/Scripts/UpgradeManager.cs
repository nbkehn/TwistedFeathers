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
    public Player upgrader1;
    public Player upgrader2;

    //Skills to pick
    List<Skill> pickUs;

    public bool play2;
    //is Player in upgrade phase?
    public bool done;
    //buttons for upgrade options
    public Button u1,u2,u3,u4;
    //reroll button
    public Button Re;
    private bool rerolled = false;

    //id of current upgrading player
    public Text playID;

    //Strings of Skill options on buttons
    public string opt1, opt2, opt3, opt4;
    //Strings of enemy skill options on buttons
    public string opte1, opte2, opte3, opte4, optReE;
    public string constRe = "Reroll - ";
    public string dash = " - ";
    //needs to:
    //  get the player's current skills
    //  get the pool of available skills
    //  decide skills to offer based off of criteria

    private void Start()
    {
        upgrader1 = GameManager.Player_db["person A"];
        upgrader2 = GameManager.Player_db["person B"];
        //Debug.Log("Player db info should be right below this******");
        //List<string> keys = GameManager.Player_db.Keys.ToList();
        //Debug.Log("Keys: <" + keys[0] + ">, <" + keys[1] + ">");
        //Debug.Log("Player_db size = " + GameManager.Player_db.Count);
        Debug.Log("Upgrader1: " + upgrader1.Name);
        Debug.Log("Upgrader2: " + upgrader2.Name);
        play2 = false;
        done = false;
        playID.text = "Player 1";
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
        Debug.Log("Populate is Called. Player 2 status: " + play2);
        //retrieve skill pool
        //fetch 4 skills from pool
        //  add criteria later
        //parse names, combine them with enemy skills
        //set names to text of buttons b1-b4
        Dictionary<string, Skill> skillPool = GameManager.Skill_db;
        
        List<Skill> foundPlayerSkills = new List<Skill>();
        List<Skill> foundEnemySkills = new List<Skill>();
        pickUs = Enumerable.ToList(skillPool.Values);
        if (!play2)
        {
            Skill[] available = upgrader1.SkillTree;
            while (foundPlayerSkills.Count < 4)
            {
                int i = (int)Random.Range(0, available.Length - 1);
                //XXX = ; //Random between 0-size
                if (!upgrader1.Skills.Contains(available[i]))
                {
                    foundPlayerSkills.Add(available[i]);
                }
                //if (pickUs[i].User_type == p_type.player)
                //{
                //    if (!foundPlayerSkills.Contains(pickUs[i]))
                //    {
                //        foundPlayerSkills.Add(pickUs[i]);
                //    }
                //}
            }
        }
        else
        {
            Skill[] available = upgrader2.SkillTree;
            while (foundPlayerSkills.Count < 4)
            {
                int i = (int)Random.Range(0, available.Length - 1);
                //XXX = ; //Random between 0-size
                if (!upgrader2.Skills.Contains(available[i]))
                {
                    foundPlayerSkills.Add(available[i]);
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
        u1.GetComponentInChildren<Text>().text = opt1 + dash + opte1;
        u2.GetComponentInChildren<Text>().text = opt2 + dash + opte2;
        u3.GetComponentInChildren<Text>().text = opt3 + dash + opte3;
        u4.GetComponentInChildren<Text>().text = opt4 + dash + opte4;
        Re.GetComponentInChildren<Text>().text = constRe + optReE;
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
        string[] skillName;
        string[] splitter = new string[] { dash };
        //learn skill based off name
        
        if (!done)
        {
            skillName = name.GetComponent<Text>().text.Split(splitter, System.StringSplitOptions.None);
            Debug.Log("Skill upgrader is looking for: " + skillName[0]);
            Debug.Log("Skill Enemy is looking for: " + skillName[1]);
            List<Monster> enemies = GameManager.Monster_db.Values.ToList();
            for (int i = 0; i < pickUs.Count; i++)
            {
                if(pickUs[i].Name == skillName[0])
                {
                    Debug.Log("Skill found "+ pickUs[i].Name);
                    if (!play2)
                    {
                        upgrader1.AddSkill(pickUs[i]);
                        if (pickUs[i].SkillType == Skill_Type.Attack)
                        {
                            upgrader1.addAttack(pickUs[i]);
                            upgrader1.AddSkill(pickUs[i]);
                        }
                        else if (pickUs[i].SkillType == Skill_Type.Utility)
                        {
                            upgrader1.addUtility(pickUs[i]);
                            upgrader1.AddSkill(pickUs[i]);
                        }
                        else if (pickUs[i].SkillType == Skill_Type.Passive)
                        {
                            upgrader1.addPassive(pickUs[i]);
                            upgrader1.AddSkill(pickUs[i]);
                        }
                    }
                    else
                    {
                        upgrader2.AddSkill(pickUs[i]);
                        if (pickUs[i].SkillType == Skill_Type.Attack)
                        {
                            upgrader2.addAttack(pickUs[i]);
                            upgrader2.AddSkill(pickUs[i]);
                        }
                        else if (pickUs[i].SkillType == Skill_Type.Utility)
                        {
                            upgrader2.addUtility(pickUs[i]);
                            upgrader2.AddSkill(pickUs[i]);
                        }
                        else if (pickUs[i].SkillType == Skill_Type.Passive)
                        {
                            upgrader2.addPassive(pickUs[i]);
                            upgrader2.AddSkill(pickUs[i]);
                        }
                    }
                    
                    break;
                }
                if(pickUs[i].Name == skillName[1])
                {
                    //give the enemy their skill
                    foreach(Monster enemy in enemies)
                    {
                        //Debug.Log(enemy.SkillTree[0].Name);
                        Debug.Log("Enemy being upgraded: " + enemy.Name);
                        if(enemy == null)
                        {
                            Debug.Log("The enemy");
                        } else if(enemy.SkillTree == null)
                        {
                            Debug.Log("Skill tree");
                        } else if (enemy.SkillTree[0] == null)
                        {
                            Debug.Log("Skill");
                        } else if(enemy.SkillTree[0].Name == null)
                        {
                            Debug.Log("Name");
                        }
                        if (enemy.SkillTree.Contains<Skill>(pickUs[i]))
                        {
                            if(pickUs[i].SkillType == Skill_Type.Attack)
                            {
                                enemy.addAttack(pickUs[i]);
                                enemy.AddSkill(pickUs[i]);
                            } else if (pickUs[i].SkillType == Skill_Type.Utility)
                            {
                                enemy.addUtility(pickUs[i]);
                                enemy.AddSkill(pickUs[i]);
                            }
                            else if(pickUs[i].SkillType == Skill_Type.Passive)
                            {
                                enemy.addPassive(pickUs[i]);
                                enemy.AddSkill(pickUs[i]);
                            }
                        }
                    }
                }
            }
            //Skill picked = pickUs.
            done = true;
            Debug.Log("Should have chosen skill by now");
            if (play2)
            {
                Invoke("FinishUpgrade", 1f);
            }
            else
            {
                Invoke("Player2Upgrade", 1f);
            }
            

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

    private void Player2Upgrade()
    {
        done = false;
        play2 = true;
        playID.text = "Player 2";
        populate();
    }

    private void FinishUpgrade()
    {
        SceneManager.LoadScene("TestScene");
    }
}
