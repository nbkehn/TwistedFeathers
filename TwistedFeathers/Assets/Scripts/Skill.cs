using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    //name of skill
    private string name;
    private string description;
    //participant type
    private p_type part_type;
    bool unlocked;
    //whether skill is upgradeable
    bool upgradeable;
    //level requirement for skill
    int level_req;
    Skill pre_req; 
    private BattleEffect effect;

    /**
     *Basic Constructor
     *
     */
    public Skill()
    {
        this.name = "";
        this.description = null;
        this.part_type = p_type.none;
        this.unlocked = false;
        this.level_req = 0;
        this.pre_req = null;
        this.effect = new BattleEffect();
        this.upgradeable = false;
    }


    /**
     *Advanced constructor for specific skills
     * @param name skill name
     * @param description skill description (what it does)
     * @param user_type what participants can use this skill
     * @param effect effect of skill
     * @param upgradeable can skill be upgraded, true if yes, false otherwise
     *
     *
     */
    public Skill(string name, string description, p_type part_type, BattleEffect effect, bool upgradeable)
    {
        this.name = name;
        this.description = description;
        this.part_type = part_type;
        this.unlocked = false;
        this.level_req = 0;
        this.pre_req = null;
        this.effect = effect;
        this.upgradeable = upgradeable;
    }


    //getters and setters
    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public p_type Part_type { get => part_type; set => part_type = value; }
    public BattleEffect Effect { get => effect; set => effect = value; }
    public bool Unlocked { get => unlocked; set => unlocked = value; }
    public int Level_req { get => level_req; set => level_req = value; }
    public Skill Pre_req { get => pre_req; set => pre_req = value; }
}
