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
    //List<Skill> pickUs;

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
    public string constRe = "Reroll";
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
        Debug.Log("Player 1!!");
        playID.text = "Player 1 Upgrades";
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
        //pickUs = Enumerable.ToList(skillPool.Values);
        if (!play2)
        {
            Skill[] available = upgrader1.SkillTree;
            while (foundPlayerSkills.Count < 4)
            {
                int i = (int)Random.Range(0, available.Length - 1);
                //XXX = ; //Random between 0-size
                if (!upgrader1.Skills.Contains(available[i]))
                {
                    if (available[i].Pre_req != null)
                    {
                        Debug.Log("Pre-req skill chosen. Skill: " + available[i].Name);
                        if (upgrader1.Skills.Contains(available[i].Pre_req))
                        {
                            foundPlayerSkills.Add(available[i]);
                        }
                    }
                    else
                    {
                        foundPlayerSkills.Add(available[i]);
                    }

                }
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
                    if(available[i].Pre_req != null)
                    {
                        if (upgrader2.Skills.Contains(available[i].Pre_req))
                        {
                            foundPlayerSkills.Add(available[i]);
                        }
                    }
                    else
                    {
                        foundPlayerSkills.Add(available[i]);
                    }
                }
            }

        }
        List<Monster> enemies = GameManager.Monster_db.Values.ToList();
        List<Skill> availableESkills = new List<Skill>();
        foreach(Monster enemy in enemies)
        {
            foreach(Skill skill in enemy.SkillTree)
            {
                if(!enemy.Skills.Contains(skill) || skill.Repeatable)
                {
                    availableESkills.Add(skill);
                }
            }
        }
        while (foundEnemySkills.Count < 5)
        {
            int i = (int)Random.Range(0, availableESkills.Count - 1);
            if (!foundEnemySkills.Contains(availableESkills[i]) || availableESkills[i].Repeatable)
            {
                foreach (Monster enemy in enemies)
                {
                    if (enemy.SkillTree.Contains(availableESkills[i]))
                    {
                        if (!enemy.Skills.Contains(availableESkills[i]) || availableESkills[i].Repeatable)
                        {
                            if (availableESkills[i].Pre_req != null)
                            {
                                if (enemy.Skills.Contains(availableESkills[i].Pre_req))
                                {
                                    foundEnemySkills.Add(availableESkills[i]);
                                    break;
                                }
                            }
                            else
                            {
                                foundEnemySkills.Add(availableESkills[i]);
                            }
                        }
                    }
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
        string pskillName;
        string eskillName;
        string[] splitter = new string[] { dash };
        //learn skill based off name
        Debug.Log(name);
        if (!done)
        {
            pskillName = name.transform.GetChild(0).GetComponentInChildren<Text>().text;
            eskillName = name.transform.GetChild(1).GetComponentInChildren<Text>().text;
            //skillName = name.GetComponent<Text>().text.Split(splitter, System.StringSplitOptions.None);
            Debug.Log("Skill upgrader is looking for: " + pskillName);
            Debug.Log("Skill Enemy is looking for: " + eskillName);
            List<Monster> enemies = GameManager.Monster_db.Values.ToList();
            Skill[] available;
            if (!play2)
            {
                available = upgrader1.SkillTree;
            }
            else
            {
                available = upgrader2.SkillTree;
            }

            foreach(Skill skill in available)
            {
                if(skill.Name == pskillName)
                {
                    Debug.Log("Skill found "+ skill.Name);
                    if (!play2)
                    {
                        if(skill.Pre_req != null)
                        {
                            Debug.Log("Attempting to remove pre-req skill");
                            if(skill.Pre_req.SkillType == Skill_Type.Attack)
                            {
                                Debug.Log("Removing Attack");
                                upgrader1.removeAttack(skill.Pre_req);
                            } else if(skill.Pre_req.SkillType == Skill_Type.Utility)
                            {
                                Debug.Log("Removing Utility");
                                upgrader1.removeUtility(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Passive)
                            {
                                Debug.Log("Removing Passive");
                                upgrader1.removePassive(skill.Pre_req);
                            }
                            upgrader1.RemoveSkill(skill.Pre_req);
                        }
                        else
                        {
                            foreach(Skill doubleCheck in upgrader1.Skills)
                            {
                                if(skill == doubleCheck.Pre_req)
                                {
                                    Debug.Log("Removing the upgraded skill to replace it with the pre-req ");
                                    if (skill.Pre_req.SkillType == Skill_Type.Attack)
                                    {
                                        Debug.Log("Removing Attack");
                                        upgrader1.removeAttack(doubleCheck);
                                    }
                                    else if (skill.Pre_req.SkillType == Skill_Type.Utility)
                                    {
                                        Debug.Log("Removing Utility");
                                        upgrader1.removeUtility(doubleCheck);
                                    }
                                    else if (skill.Pre_req.SkillType == Skill_Type.Passive)
                                    {
                                        Debug.Log("Removing Passive");
                                        upgrader1.removePassive(doubleCheck);
                                    }
                                    upgrader1.RemoveSkill(doubleCheck);
                                }
                            }
                        }
                        if (upgrader1.Skills.Contains(skill) && skill.Repeatable)//if repeatable passive
                        {
                            bool duplicate = true;
                            while (duplicate)
                            {
                                bool foundOne = false;
                                skill.Name = skill.Name + "+";
                                foreach(Skill checkMe in upgrader1.SkillTree)
                                {
                                    if(checkMe.Name == skill.Name)
                                    {
                                        foundOne = true;
                                    }
                                }
                                if (!foundOne)
                                {
                                    duplicate = false;
                                }
                            }
                            upgrader1.AddSkill(skill);
                            if (skill.SkillType == Skill_Type.Attack)
                            {
                                upgrader1.addAttack(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Utility)
                            {
                                upgrader1.addUtility(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Passive)
                            {
                                upgrader1.addPassive(skill);
                            }
                        }
                        else
                        {
                            upgrader1.AddSkill(skill);
                            if (skill.SkillType == Skill_Type.Attack)
                            {
                                upgrader1.addAttack(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Utility)
                            {
                                upgrader1.addUtility(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Passive)
                            {
                                upgrader1.addPassive(skill);
                            }
                        }
                        
                    }
                    else
                    {
                        if (skill.Pre_req != null)
                        {
                            Debug.Log("Attempting to remove pre-req skill");
                            if (skill.Pre_req.SkillType == Skill_Type.Attack)
                            {
                                Debug.Log("Removing Attack");
                                upgrader2.removeAttack(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Utility)
                            {
                                Debug.Log("Removing Utility");
                                upgrader2.removeUtility(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Passive)
                            {
                                Debug.Log("Removing Passive");
                                upgrader2.removePassive(skill.Pre_req);
                            }
                            upgrader2.RemoveSkill(skill.Pre_req);
                        }
                        else
                        {
                            foreach (Skill doubleCheck in upgrader2.Skills)
                            {
                                if (skill == doubleCheck.Pre_req)
                                {
                                    Debug.Log("Removing the upgraded skill to replace it with the pre-req ");
                                    if (skill.Pre_req.SkillType == Skill_Type.Attack)
                                    {
                                        Debug.Log("Removing Attack");
                                        upgrader2.removeAttack(doubleCheck);
                                    }
                                    else if (skill.Pre_req.SkillType == Skill_Type.Utility)
                                    {
                                        Debug.Log("Removing Utility");
                                        upgrader2.removeUtility(doubleCheck);
                                    }
                                    else if (skill.Pre_req.SkillType == Skill_Type.Passive)
                                    {
                                        Debug.Log("Removing Passive");
                                        upgrader2.removePassive(doubleCheck);
                                    }
                                    upgrader2.RemoveSkill(doubleCheck);
                                }
                            }
                        }
                        if (upgrader2.Skills.Contains(skill) && skill.Repeatable)//if repeatable passive
                        {
                            bool duplicate = true;
                            while (duplicate)
                            {
                                bool foundOne = false;
                                skill.Name = skill.Name + "+";
                                foreach (Skill checkMe in upgrader2.SkillTree)
                                {
                                    if (checkMe.Name == skill.Name)
                                    {
                                        foundOne = true;
                                    }
                                }
                                if (!foundOne)
                                {
                                    duplicate = false;
                                }
                            }
                            upgrader2.AddSkill(skill);
                            if (skill.SkillType == Skill_Type.Attack)
                            {
                                upgrader2.addAttack(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Utility)
                            {
                                upgrader2.addUtility(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Passive)
                            {
                                upgrader2.addPassive(skill);
                            }
                        }
                        else
                        {
                            upgrader2.AddSkill(skill);
                            if (skill.SkillType == Skill_Type.Attack)
                            {
                                upgrader2.addAttack(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Utility)
                            {
                                upgrader2.addUtility(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Passive)
                            {
                                upgrader2.addPassive(skill);
                            }
                        }
                    }
                    
                    break;
                }
            }
          
            //give the enemy their skill
            foreach (Monster enemy in enemies)
            {

                foreach (Skill skill in enemy.SkillTree)
                {
                    if (skill.Name == eskillName)
                    {

                        Debug.Log("*******Skill Enemy is receiving: " + skill.Name);
                        if (skill.Pre_req != null)
                        {
                            Debug.Log("Attempting to remove pre-req skill");
                            if (skill.Pre_req.SkillType == Skill_Type.Attack)
                            {
                                Debug.Log("Removing Attack");
                                enemy.removeAttack(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Utility)
                            {
                                Debug.Log("Removing Utility");
                                enemy.removeUtility(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Passive)
                            {
                                Debug.Log("Removing Passive");
                                enemy.removePassive(skill.Pre_req);
                            }
                            enemy.RemoveSkill(skill.Pre_req);
                        }
                        if (enemy.Skills.Contains(skill) && skill.Repeatable)//if repeatable passive
                        {
                            int loopcount = 0;
                            bool duplicate = true;
                            Skill skill2 = new Skill(skill.Name, skill.Description, skill.User_type, skill.Effects);
                            while (duplicate && loopcount < 10)
                            {
                                bool foundOne = false;
                                skill2.Name = skill2.Name + "+";
                                
                                foreach (Skill checkMe in enemy.Skills)
                                {
                                    if (checkMe.Name == skill2.Name)
                                    {
                                        Debug.Log("Dupe found: " + checkMe.Name);
                                        Debug.Log("Skill being checked against: " + skill2.Name);
                                        foundOne = true;
                                    }
                                }
                                if (!foundOne)
                                {
                                    duplicate = false;
                                }
                                loopcount++;
                            }
                            
                            Debug.Log("Loopcount is " + loopcount);
                            enemy.AddSkill(skill2);
                            if (skill2.SkillType == Skill_Type.Attack)
                            {
                                enemy.addAttack(skill2);
                            }
                            else if (skill2.SkillType == Skill_Type.Utility)
                            {
                                enemy.addUtility(skill2);
                            }
                            else if (skill.SkillType == Skill_Type.Passive)
                            {
                                enemy.addPassive(skill2);
                            }
                        }
                        else
                        {
                            enemy.AddSkill(skill);
                            if (skill.SkillType == Skill_Type.Attack)
                            {
                                enemy.addAttack(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Utility)
                            {
                                enemy.addUtility(skill);
                            }
                            else if (skill.SkillType == Skill_Type.Passive)
                            {
                                enemy.addPassive(skill);
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


    public void Reroll(GameObject name)
    {
        List<Monster> enemies = GameManager.Monster_db.Values.ToList();
        string eskillName = name.transform.GetChild(1).GetComponentInChildren<Text>().text;
        //give the enemy their skill
        foreach (Monster enemy in enemies)
        {

            foreach (Skill skill in enemy.SkillTree)
            {
                if (skill.Name == eskillName)
                {

                    Debug.Log("*******Skill Enemy is receiving via Reroll: " + skill.Name);
                    if (enemy.SkillTree.Contains(skill))
                    {
                        if (skill.Pre_req != null)
                        {
                            Debug.Log("Attempting to remove pre-req skill");
                            if (skill.Pre_req.SkillType == Skill_Type.Attack)
                            {
                                Debug.Log("Removing Attack");
                                enemy.removeAttack(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Utility)
                            {
                                Debug.Log("Removing Utility");
                                enemy.removeUtility(skill.Pre_req);
                            }
                            else if (skill.Pre_req.SkillType == Skill_Type.Passive)
                            {
                                Debug.Log("Removing Passive");
                                enemy.removePassive(skill.Pre_req);
                            }
                            enemy.RemoveSkill(skill.Pre_req);
                        }
                        enemy.AddSkill(skill);
                        if (skill.SkillType == Skill_Type.Attack)
                        {
                            enemy.addAttack(skill);
                        }
                        else if (skill.SkillType == Skill_Type.Utility)
                        {
                            enemy.addUtility(skill);
                        }
                        else if (skill.SkillType == Skill_Type.Passive)
                        {
                            enemy.addPassive(skill);
                        }
                    }
                }
            }
        }
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
        rerolled = false;
        playID.text = "Player 2 Upgrades";
        populate();
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
