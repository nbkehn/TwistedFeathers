public enum Effect { None, Burn, Damage, Poison }

[System.Serializable]
public class Skill {

    public int id_Skill;
    public string name;
    public string description;
    public Effect effect;
    public int pre_req;
    public bool unlocked;
    public int level_req;
}
