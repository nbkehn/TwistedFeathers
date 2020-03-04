using System.Collections.Generic;

[System.Serializable]
public class Skill {

    public int id_Skill;
    public string name;
    public string description;
    public int pre_req;
    public bool unlocked;
    public int level_req;
    public List<BattleEffect> effects;
    public List<BattleEffect> passives;

    public Skill(int id)
    {
        id_Skill = id;
        effects = new List<BattleEffect>();
        passives = new List<BattleEffect>();
    }
}
