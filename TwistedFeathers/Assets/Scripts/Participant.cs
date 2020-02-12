using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum p_type {none, player, enemy, environment, weather};

public abstract class Participant
{
    private p_type type;
    private string name;
    private ArrayList skills;

    public Participant()
    {
        this.type = p_type.none;
        this.name = "";
        this.skills = new ArrayList();
    }

    public Participant(p_type type, string name)
    {
        this.type = type;
        this.name = name;
        this.skills = new ArrayList();
    }

    public p_type Type { get => type; set => type = value; }
    public ArrayList Skills { get => skills; set => skills = value; }
    public string Name { get => name; set => name = value; }

    public void AddSkill(Skill new_skill)
    {
        this.skills.Add(new_skill);
    }
}

