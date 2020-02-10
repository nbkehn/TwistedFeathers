using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum p_type {none, player, enemy, environment, weather};

public abstract class Participant
{
    private p_type type;
    private ArrayList skills;

    public Participant()
    {
        this.type = p_type.none;
        this.skills = new ArrayList();
    }

    public Participant(p_type type)
    {
        this.type = type;
        this.skills = new ArrayList();
    }

    public p_type Type { get => type; set => type = value; }
    public ArrayList Skills { get => skills; set => skills = value; }

    public void AddSkill(Skill new_skill)
    {
        this.skills.Add(new_skill);
    }
}


public class Participant_B : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
