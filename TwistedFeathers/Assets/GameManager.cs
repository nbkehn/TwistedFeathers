using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static Dictionary<string, Skill> skill_db;
    static Dictionary<string, Participant> participant_db;

    static public Dictionary<string, Skill> Skill_db { get => skill_db; set => skill_db = value; }
    static public Dictionary<string, Participant> Participant_db { get => participant_db; set => participant_db = value; }

    // Awake is called before the first frame update and before Starts
    void Awake()
    {
        Skill_db = new Dictionary<string, Skill>();
        Participant_db = new Dictionary<string, Participant>();

        Skill_db.Add("dummy A", new Skill("Test Skill", "Does nothing", p_type.none, new BattleEffect(e_type.nothing, "This is A dummy", 1)));
        Skill_db.Add("dummy B", new Skill("Test Skill", "Does nothing", p_type.none, new BattleEffect(e_type.nothing, "This is B dummy", 0)));

        Participant_db.Add("person A", new Player());
        Participant_db["person A"].AddSkill(Skill_db["dummy A"]);
        Participant_db.Add("person B", new Player());
        Participant_db["person B"].AddSkill(Skill_db["dummy B"]);

    }

    // Update is called once per frame
    void Update()
    {

    }
}


