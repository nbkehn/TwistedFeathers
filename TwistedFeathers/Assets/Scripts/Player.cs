using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Participant
{
    private Dictionary<string, Skill> learnedSkills;
    public Player() : base(p_type.player, "")
    {

    }

    public Player(string name) : base(p_type.player, name)
    {

    }

    //public Dictionary<string, Skill> skill_db getSkillList()
    //{
    //    return this.learnedSkills;
    //}
}


