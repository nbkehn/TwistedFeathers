using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Participant
{
    private Dictionary<string, Skill> learnedSkills = new Dictionary<string, Skill>();
    public Player() : base(p_type.player, "")
    {

    }

    public Player(string name) : base(p_type.player, name)
    {

    }

    public Dictionary<string, Skill> getSkillList()
    {
        return this.learnedSkills;
    }

    public void learnSkill(Skill skill)
    {
        learnedSkills.Add(skill.Name, skill);
    }
}


