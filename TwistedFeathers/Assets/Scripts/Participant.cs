using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum p_type {none, player, enemy, environment, weather};

public abstract class Participant
{
    private p_type type;
    private int max_hp;
    private int current_hp;
    private float defense;
    private float attack;
    private float dodge;
    private string name;
    private List<Skill> skills;

    public Participant()
    {
        this.type = p_type.none;
        this.name = "";
        this.max_hp = 50;
        this.current_hp = 50;
        this.defense = 0.0f;
        this.attack = 0.0f;
        this.dodge = 0.0f;
        this.skills = new List<Skill>();
    }

    public Participant(p_type type, string name)
    {
        this.type = type;
        this.name = name;
        this.max_hp = 50;
        this.current_hp = 50;
        this.defense = 0.0f;
        this.attack = 0.0f;
        this.dodge = 0.0f;
        this.skills = new List<Skill>();
    }

    public p_type Type { get => type; set => type = value; }
    public List<Skill> Skills { get => skills; set => skills = value; }
    public string Name { get => name; set => name = value; }
    public int Max_hp { get => max_hp; set => max_hp = value; }
    public int Current_hp { get => current_hp; set => current_hp = value; }
    public float Defense { get => defense; set => defense = value; }
    public float Attack { get => attack; set => attack = value; }
    public float Dodge { get => dodge; set => dodge = value; }

    public void AddSkill(Skill new_skill)
    {
        this.skills.Add(new_skill);
    }
}

