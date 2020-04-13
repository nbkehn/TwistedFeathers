using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TwistedFeathers;

public class GameManager : MonoBehaviour
{
    static Dictionary<string, Skill> skill_db;
    static Dictionary<string, Participant> participant_db;

    static Dictionary<string, Skill> enemySkills;
    static public Dictionary<string, Skill> eLearnedSkills { get => enemySkills; set => enemySkills = value; }

    static Dictionary<string, Monster> monster_db;
    static Dictionary<string, Player> player_db;

    static public Dictionary<string, Skill> Skill_db { get => skill_db; set => skill_db = value; }
    static public Dictionary<string, Participant> Participant_db { get => participant_db; set => participant_db = value; }
    public static Dictionary<string, Monster> Monster_db { get => monster_db; set => monster_db = value; }
    public static Dictionary<string, Player> Player_db { get => player_db; set => player_db = value; }


    static public bool singlePlayer = false;


    public GameObject combater;
    bool inCombat;
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    

    public List<GameObject> environmentPrefabs;
    public static List<Environment> environments;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

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
            eLearnedSkills = new Dictionary<string, Skill>();
            inCombat = false;
            Monster_db = new Dictionary<string, Monster>();

            //Dummy values for testing purposes
            Skill_db.Add("dummy A", new Skill("Acid", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy B", new Skill("Poison", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy C", new Skill("Health Inc", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy D", new Skill("Fire Breath", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy E", new Skill("Claws", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy F", new Skill("Enraged", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            //player skills
            //Skill_db.Add("FeatherDagger", new Skill("Feather Dagger", "Deals 20 damage", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 20f, "Feather Dagger") }));
            //Skill_db.Add("Sabotage", new Skill("Sabotage", "Reduce enemy defense by 25% for 1 turn", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.buff, -.25f, 3, "defense") }));
            Skill_db.Add("DefensiveFeathers", new Skill("Defensive Feathers", "Increase defense by 10%", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.buff, .1f, 3, "defense") }));
            Skill_db.Add("smarty A", new Skill("Dagger", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty B", new Skill("Pollen Bombs", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty C", new Skill("Hover", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty D", new Skill("Flight", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty E", new Skill("Nectar Drunk", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty F", new Skill("Swift Wings", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty G", new Skill("Distraction", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty H", new Skill("Poison Rain", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty I", new Skill("Attack2", "Does nothing", p_type.player, new List<BattleEffect>()));
            Skill_db.Add("smarty J", new Skill("Attack3", "Does nothing", p_type.player, new List<BattleEffect>()));





            //Dummy values for testing purposes

            Skill_db.Add("Azazel's Skill", new Skill("Azazel's Skill", "Does nothing", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.nothing, 0f, 0, "This is A dummy") }));
            Skill_db.Add("Beelzebub's Skill", new Skill("Beelzebub's Skill", "Deals 10 damage", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.damage, 10f, "This is B dummy") }));
            Skill_db.Add("Adam's Skill", new Skill("Adam's Skill", "Does 15 damage", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 15f, "This is A smarty") }));
            Skill_db.Add("Ben's Skill", new Skill("Ben's Skill", "Deals 10 damage", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 10f, "This is B smarty") }));

            Player_db.Add("person A", new Player(s_type.Rogue));
            //Player_db["person A"].LoadSkillTree();
            //Player_db["person A"].AddSkill(Skill_db["Adam's Skill"]);
            //Player_db["person A"].AddSkill(Skill_db["Sabotage"]);
            //Player_db["person A"].AddSkill(Skill_db["DefensiveFeathers"]);
            //Player_db["person A"].AddSkill(Skill_db["FeatherDagger"]);
            Player_db["person A"].myPrefab = playerPrefab;
            Player_db.Add("person B", new Player(s_type.Rogue));
            Player_db["person B"].AddSkill(Skill_db["Ben's Skill"]);
            Player_db["person B"].myPrefab = playerPrefab;

            Monster_db.Add("enemy A", new Monster(s_type.Necromancer));
            Monster_db["enemy A"].AddSkill(Skill_db["Azazel's Skill"]);
            Monster_db["enemy A"].myPrefab = enemyPrefab;
            Monster_db.Add("enemy B", new Monster(s_type.Thief));
            Monster_db["enemy B"].AddSkill(Skill_db["Beelzebub's Skill"]);
            Monster_db["enemy B"].myPrefab = enemyPrefab;

            environments = new List<Environment>();

            Skill environment_do_nothing = new Skill("Nothing", "Environment does nothing", p_type.environment, new List<BattleEffect>());

            List<KeyValuePair<int, Skill>> desert_skills = new List<KeyValuePair<int, Skill>>() 
            { 
                new KeyValuePair<int, Skill>
                (2,
                    new Skill("Mirage", "Decreases Accuracy for all Battle Participants", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.buff, -0.25f, 3, "accuracy")
                    })
                ),
                new KeyValuePair<int, Skill>(1, environment_do_nothing)
            };
            List<KeyValuePair<int, Skill>> swamp_skills = new List<KeyValuePair<int, Skill>>() 
            {
                new KeyValuePair<int, Skill>
                (2,
                    new Skill("Leeches", "Leeches latch onto you and deal minor damage for 3 turns", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.damage, 1, 0, "", 0),
                        new BattleEffect(e_type.damage, 1, 0, "", 1),
                        new BattleEffect(e_type.damage, 1, 0, "", 2)
                    })
                ),
                new KeyValuePair<int, Skill>(1, environment_do_nothing)
            };

            environments.Add(new Environment(s_type.Desert,environmentPrefabs[0], desert_skills));
            environments.Add(new Environment(s_type.Swamp,environmentPrefabs[1], swamp_skills));
            environments.Add(new Environment(s_type.None,environmentPrefabs[2]));
        }
    }

    public void onSinglePlayer()
    {
        singlePlayer = true;
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


