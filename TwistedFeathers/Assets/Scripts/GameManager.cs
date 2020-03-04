﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwistedFeathers;

public class GameManager : MonoBehaviour
{
    static Dictionary<string, Skill> skill_db;
    static Dictionary<string, Participant> participant_db;
    static Dictionary<string, Monster> monster_db;
    static Dictionary<string, Player> player_db;

    static public Dictionary<string, Skill> Skill_db { get => skill_db; set => skill_db = value; }
    static public Dictionary<string, Participant> Participant_db { get => participant_db; set => participant_db = value; }
    public static Dictionary<string, Monster> Monster_db { get => monster_db; set => monster_db = value; }
    public static Dictionary<string, Player> Player_db { get => player_db; set => player_db = value; }


    public GameObject combater;
    bool inCombat;
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    

    public List<GameObject> environmentPrefabs;
    public static List<Environment> environments;

    public bool rotate = true;

    // Awake is called before the first frame update and before Starts
    void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        

            Skill_db = new Dictionary<string, Skill>();
            Participant_db = new Dictionary<string, Participant>();
            Player_db = new Dictionary<string, Player>();
            Monster_db = new Dictionary<string, Monster>();
            inCombat = false;

            //Dummy values for testing purposes
            Skill_db.Add("dummy A", new Skill("Test Skill", "Does nothing", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.nothing, 0f, "This is A dummy") }));
            Skill_db.Add("dummy B", new Skill("Test Skill", "Does nothing", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.damage, 20f, "This is B dummy") }));
            Skill_db.Add("smarty A", new Skill("Test Skill", "Does nothing", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.nothing, 0f, "This is A smarty") }));
            Skill_db.Add("smarty B", new Skill("Test Skill", "Does nothing", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 20f, "This is B smarty") }));

            Player_db.Add("person A", new Player("Adam"));
            Player_db["person A"].AddSkill(Skill_db["smarty A"]);
            Player_db.Add("person B", new Player("Ben"));
            Player_db["person B"].AddSkill(Skill_db["smarty B"]);

            Monster_db.Add("enemy A", new Monster("Azazel"));
            Monster_db["enemy A"].AddSkill(Skill_db["dummy A"]);
            Monster_db.Add("enemy B", new Monster("Beelzebub"));
            Monster_db["enemy B"].AddSkill(Skill_db["dummy B"]);
            environments = new List<Environment>();

            List<Skill> desert_skills = new List<Skill>() { };
            List<Skill> swamp_skills = new List<Skill>() { };

            environments.Add(new Environment("desert",environmentPrefabs[0], desert_skills));
            environments.Add(new Environment("swamp",environmentPrefabs[1], swamp_skills));
            environments.Add(new Environment("empty",environmentPrefabs[2]));
        }
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


