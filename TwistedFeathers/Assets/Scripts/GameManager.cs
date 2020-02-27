using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static Dictionary<string, Skill> skill_db;
    static Dictionary<string, Participant> participant_db;
    //list of players would be helpful
    static Dictionary<string, Player> player_db;
    static Dictionary<string, Skill> enemySkills;

    static public Dictionary<string, Skill> Skill_db { get => skill_db; set => skill_db = value; }
    static public Dictionary<string, Participant> Participant_db { get => participant_db; set => participant_db = value; }
    static public Dictionary<string, Player> Player_db { get => player_db; set => player_db = value; }
    static public Dictionary<string, Skill> eLearnedSkills { get => enemySkills; set => enemySkills = value; }

    public GameObject combater;
    bool inCombat;

    // Awake is called before the first frame update and before Starts
    void Awake()
    {
        Skill_db = new Dictionary<string, Skill>();
        Participant_db = new Dictionary<string, Participant>();
        inCombat = false;

        //Dummy values for testing purposes
        Skill_db.Add("dummy A", new Skill("Acid", "Does nothing", p_type.enemy, new BattleEffect(e_type.nothing, "This is A dummy", 0), false));
        Skill_db.Add("dummy B", new Skill("Poison", "Does nothing", p_type.enemy, new BattleEffect(e_type.damage, "This is B dummy", 0), false));
        Skill_db.Add("dummy C", new Skill("Health Inc", "Does nothing", p_type.enemy, new BattleEffect(e_type.nothing, "This is C dummy", 0), false));
        Skill_db.Add("dummy D", new Skill("Fire Breath", "Does nothing", p_type.enemy, new BattleEffect(e_type.damage, "This is D dummy", 0), false));
        Skill_db.Add("dummy E", new Skill("Claws", "Does nothing", p_type.enemy, new BattleEffect(e_type.nothing, "This is E dummy", 0), false));
        Skill_db.Add("dummy F", new Skill("Enraged", "Does nothing", p_type.enemy, new BattleEffect(e_type.damage, "This is F dummy", 0), false));
        //player skills
        Skill_db.Add("smarty A", new Skill("Dagger", "Does nothing", p_type.player, new BattleEffect(e_type.nothing, "This is A smarty", 0), false));
        Skill_db.Add("smarty B", new Skill("Pollen Bombs", "Does nothing", p_type.player, new BattleEffect(e_type.damage, "This is B smarty", 0), false));
        Skill_db.Add("smarty C", new Skill("Hover", "Does nothing", p_type.player, new BattleEffect(e_type.nothing, "This is C smarty", 0), false));
        Skill_db.Add("smarty D", new Skill("Flight", "Does nothing", p_type.player, new BattleEffect(e_type.damage, "This is D smarty", 0), false));
        Skill_db.Add("smarty E", new Skill("Nectar Drunk", "Does nothing", p_type.player, new BattleEffect(e_type.nothing, "This is E smarty", 0), false));
        Skill_db.Add("smarty F", new Skill("Swift Wings", "Does nothing", p_type.player, new BattleEffect(e_type.damage, "This is F smarty", 0), false));
        Skill_db.Add("smarty G", new Skill("Distraction", "Does nothing", p_type.player, new BattleEffect(e_type.nothing, "This is G smarty", 0), false));
        Skill_db.Add("smarty H", new Skill("Poison Rain", "Does nothing", p_type.player, new BattleEffect(e_type.damage, "This is H smarty", 0), false));
        Skill_db.Add("smarty I", new Skill("Attack2", "Does nothing", p_type.player, new BattleEffect(e_type.nothing, "This is I smarty", 0), false));
        Skill_db.Add("smarty J", new Skill("Attack3", "Does nothing", p_type.player, new BattleEffect(e_type.damage, "This is J smarty", 0), false));

        Participant_db.Add("person A", new Player("Adam"));
        Participant_db["person A"].AddSkill(Skill_db["smarty A"]);
        Participant_db.Add("person B", new Player("Ben"));
        Participant_db["person B"].AddSkill(Skill_db["smarty B"]);

        Player_db.Add("person A", new Player("Adam"));
        Player_db["person A"].AddSkill(Skill_db["smarty A"]);
        Player_db.Add("person B", new Player("Ben"));
        Player_db["person B"].AddSkill(Skill_db["smarty B"]);

        Participant_db.Add("enemy A", new Monster("Azazel"));
        Participant_db["enemy A"].AddSkill(Skill_db["dummy A"]);
        Participant_db.Add("enemy B", new Monster("Beelzebub"));
        Participant_db["enemy B"].AddSkill(Skill_db["dummy B"]);

        

       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Battle Start") && !inCombat)
        {
            Instantiate(combater);
            inCombat = true;
        }

    }
}


