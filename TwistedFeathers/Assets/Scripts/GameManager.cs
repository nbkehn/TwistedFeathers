using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

using TwistedFeathers;

public class GameManager : MonoBehaviour
{
    static Dictionary<string, Skill> skill_db;
    static Dictionary<string, Participant> participant_db;

    static Dictionary<string, Skill> enemySkills;
    static public Dictionary<string, Skill> eLearnedSkills { get => enemySkills; set => enemySkills = value; }

    static Dictionary<string, Monster> monster_db;
    static Dictionary<string, Player> player_db;

    static Dictionary<string, Monster> enemy_types;


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

    public bool tutorial = true;
    public List<Sprite> playerPics;

    public static int numWaves;

    public static int numBattles;

    public static int wavesRequired = 3;

    // Awake is called before the first frame update and before Starts
    void Awake()
    {
        if(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player1.getPlayerClass() == s_type.Rogue){
            GameObject.Find("player1_pic").GetComponent<Image>().sprite = playerPics[0];
            GameObject.Find("player2_pic").GetComponent<Image>().sprite = playerPics[1];
       } else {
            GameObject.Find("player1_pic").GetComponent<Image>().sprite = playerPics[1];
            GameObject.Find("player2_pic").GetComponent<Image>().sprite = playerPics[0];
       } 

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;

            Skill_db = new Dictionary<string, Skill>();
            Participant_db = new Dictionary<string, Participant>();
            Player_db = new Dictionary<string, Player>();
            eLearnedSkills = new Dictionary<string, Skill>();
            enemy_types = new Dictionary<string, Monster>();
            inCombat = false;
            Monster_db = new Dictionary<string, Monster>();

            //Dummy values for testing purposes
            Skill_db.Add("dummy A", new Skill("Poison Weapons", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy B", new Skill("Poison", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy C", new Skill("Health Inc", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy D", new Skill("Fire Breath", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy E", new Skill("Claws", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            Skill_db.Add("dummy F", new Skill("Enraged", "Does nothing", p_type.enemy, new List<BattleEffect>()));
            //player skills
            //Skill_db.Add("FeatherDagger", new Skill("Feather Dagger", "Deals 20 damage", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 20f, "Feather Dagger") }));
            //Skill_db.Add("Sabotage", new Skill("Sabotage", "Reduce enemy defense by 25% for 1 turn", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.buff, -.25f, 3, "defense") }));
            //Skill_db.Add("DefensiveFeathers", new Skill("Defensive Feathers", "Increase defense by 10%", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.buff, .1f, 3, "defense") }));
            //Skill_db.Add("smarty A", new Skill("Dagger", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty B", new Skill("Pollen Bombs", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty C", new Skill("Hover", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty D", new Skill("Flight", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty E", new Skill("Nectar Drunk", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty F", new Skill("Swift Wings", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty G", new Skill("Distraction", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty H", new Skill("Poison Rain", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty I", new Skill("Attack2", "Does nothing", p_type.player, new List<BattleEffect>()));
            //Skill_db.Add("smarty J", new Skill("Attack3", "Does nothing", p_type.player, new List<BattleEffect>()));


            //Enemy Type initialization
            Monster goose = new Monster(s_type.Thief, 1);
            //Skill hiss = new Skill("Hiss", "Increases Dodge Chance", p_type.enemy, new List<BattleEffect>());
            //Skill kinfeAttack = new Skill("Knife Attack", "Deals damage", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.damage, 10f, "Knife Attack") });
            //Skill hiss2 = new Skill("Hiss", "Reduces Player's attack", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.buff, -.20f, 1, "defense") });
            //goose.addPassive(hiss);//Evasive
            //goose.addAttack(kinfeAttack);
            //goose.addUtility(hiss2);
            //enemy_types.Add("Goose", goose);
            Monster goose2 = new Monster(s_type.Thief, 1);
            //goose2.addPassive(hiss);//Evasive
            //goose2.addAttack(kinfeAttack);
            //goose2.addUtility(hiss2);
            //Monster crow = new Monster(s_type.Necromancer, 2);
            ////crow.addAttack();//Dark Magick
            ////crow.addUtility();//Healing
            ////crow.addUtility();Fear Curse
            //enemy_types.Add("Crow", crow);
            //Skill_db.Add("Hiss", new Skill("Hiss", "Increases Dodge Chance", p_type.enemy, new List<BattleEffect>()));
            //Skill_db.Add("Knife Attack", new Skill("Knife Attack", "Deals damage", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.damage, 10f, "Knife Attack") }));


            //Dummy values for testing purposes

            Skill_db.Add("Azazel's Skill", new Skill("Azazel's Skill", "Does nothing", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.nothing, 0f, 0, "This is A dummy") }));
            Skill_db.Add("Beelzebub's Skill", new Skill("Beelzebub's Skill", "Deals 10 damage", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.damage, 10f, "This is B dummy") }));
            Skill_db.Add("Adam's Skill", new Skill("Adam's Skill", "Does 15 damage", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 15f, "This is A smarty") }));
            Skill_db.Add("Ben's Skill", new Skill("Ben's Skill", "Deals 10 damage", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 10f, "This is B smarty") }));

            
            Player_db.Add("person A", new Player(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player1.getPlayerClass()));
            //Player_db["person A"].LoadSkillTree();
            //Player_db["person A"].AddSkill(Skill_db["Adam's Skill"]);
            //Player_db["person A"].AddSkill(Skill_db["Sabotage"]);
            //Player_db["person A"].AddSkill(Skill_db["DefensiveFeathers"]);
            //Player_db["person A"].AddSkill(Skill_db["FeatherDagger"]);
            Player_db["person A"].myPrefab = playerPrefab;
            Player_db.Add("person B", new Player(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player2.getPlayerClass()));
            Player_db["person B"].AddSkill(Skill_db["Ben's Skill"]);
            Player_db["person B"].myPrefab = playerPrefab;

            Monster_db.Add("enemy A", goose);
            Monster_db["enemy A"].AddSkill(Skill_db["Azazel's Skill"]);
            Monster_db["enemy A"].myPrefab = enemyPrefab;
            Monster_db.Add("enemy B", goose2);
            Monster_db["enemy B"].AddSkill(Skill_db["Beelzebub's Skill"]);
            Monster_db["enemy B"].myPrefab = enemyPrefab;

            // ADD SKILLS //
            BattleParticipant p = new Player(s_type.Rogue);
            addSkills(p);
            p = new Player(s_type.Fighter);
            addSkills(p);
            //p = new Player(s_type.Mage);
            //addSkills(p);
            p = new Monster(s_type.Necromancer);
            addSkills(p);
            p = new Monster(s_type.Thief);
            addSkills(p);
            //p = new Environment(s_type.Swamp, environmentPrefabs[2]);
            //addSkills(p);
            //p = new Environment(s_type.Desert, environmentPrefabs[2]);
            //addSkills(p);

            
            //Environment setup

            environments = new List<Environment>();

            // A do-nothing environment skill for weighted odds (chance of nothing happening)
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
                new KeyValuePair<int, Skill>
                (1,
                    new Skill("Tailwind", "Increases attack power for allies, decreases for enemies", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.buff, 0.25f, 3, "attack", target_type.AllAllies),
                        new BattleEffect(e_type.buff, -0.25f, 3, "attack", target_type.AllEnemies)

                    })
                ),
                new KeyValuePair<int, Skill>
                (1,
                    new Skill("Headwind", "Decreases attack power for allies, increases for enemies", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.buff, -0.25f, 3, "attack", target_type.AllAllies),
                        new BattleEffect(e_type.buff, 0.25f, 3, "attack", target_type.AllEnemies)

                    })
                ),
                new KeyValuePair<int, Skill>
                (2,
                    new Skill("Oasis", "Heals everyone for a small amount", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.damage, -5, 0, "", target_type.All)

                    })
                ),
                new KeyValuePair<int, Skill>(5, environment_do_nothing)
            };
            List<KeyValuePair<int, Skill>> swamp_skills = new List<KeyValuePair<int, Skill>>() 
            {
                new KeyValuePair<int, Skill>
                (3,
                    new Skill("Leeches", "Leeches latch onto you and deal minor damage for 3 turns", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.damage, 1, 0, "", 0),
                        new BattleEffect(e_type.damage, 1, 0, "", 1),
                        new BattleEffect(e_type.damage, 1, 0, "", 2)
                    })
                ),
                new KeyValuePair<int, Skill>
                (1,
                    new Skill("Swamp Spirit Aid", "A swamp spirit comes to your aid, granting allies increased attack for 2 turns", p_type.environment, new List<BattleEffect>()
                    {
                        new BattleEffect(e_type.buff, 1.0f, 2, "attack", target_type.AllAllies)
                    })
                ),
                new KeyValuePair<int, Skill>(5, environment_do_nothing)
            };

            environments.Add(new Environment(s_type.Desert,environmentPrefabs[0], desert_skills));
            environments.Add(new Environment(s_type.Swamp,environmentPrefabs[1], swamp_skills));
            environments.Add(new Environment(s_type.None,environmentPrefabs[2]));
        }
    }

    public void addSkills(Participant p)
    {
        if (p.SkillTree == null)
        {
            return;
        }
        foreach (Skill skill in p.SkillTree)
        {
            if (skill_db.ContainsKey(skill.Name))
            {
                return;
            }
            Skill_db.Add(skill.Name, skill);
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

    public void toggleTutorial(){
        tutorial = !tutorial;
    }

    public void finishBattle(int exp){
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().awardEXP(exp);
        if (!singlePlayer)
        {
            PhotonNetwork.Disconnect();
        }
        SceneManager.LoadScene("TestScene");
        this.StartCoroutine("loadHub");
    }

    public IEnumerator loadHub()
    {
        int count = 0;
        while(!(SceneManager.GetActiveScene ().name == "TestScene") && count < 4){
            //WAIT
            yield return new WaitForSeconds(.1f);
        }
        GameObject.Find("NumBattles").GetComponent<Text>().text = "" + numBattles;
        GameObject.Find("player1EXP").GetComponent<Text>().text = "EXP    " + GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player1.totalEXP;
        GameObject.Find("player2EXP").GetComponent<Text>().text = "EXP    " + GameObject.Find("PlayerManager").GetComponent<PlayerManager>().player2.totalEXP;
    }
}


