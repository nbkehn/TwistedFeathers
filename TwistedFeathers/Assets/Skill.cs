using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    private string name;
    private string description;
    private p_type user_type;
    private BattleEffect effect;

    public Skill()
    {
        this.name = "";
        this.description = null;
        this.user_type = p_type.none;
        this.effect = new BattleEffect();
    }

    public Skill(string name, string description, p_type user_type, BattleEffect effect)
    {
        this.name = name;
        this.description = description;
        this.user_type = user_type;
        this.effect = effect;
    }

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public p_type User_type { get => user_type; set => user_type = value; }
    public BattleEffect Effect { get => effect; set => effect = value; }
}



public class Skill_B : MonoBehaviour
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
