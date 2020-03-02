using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static Dictionary<string, Skill> skill_db;
    static Dictionary<string, Participant> participant_db;

    static public Dictionary<string, Skill> Skill_db { get => skill_db; set => skill_db = value; }
    static public Dictionary<string, Participant> Participant_db { get => participant_db; set => participant_db = value; }

    public GameObject combater;
    bool inCombat;
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public List<GameObject> environmentPrefabs;
    public static List<Environment> environments;

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
            inCombat = false;

            //Dummy values for testing purposes
            Skill_db.Add("dummy A", new Skill("Test Skill", "Does nothing", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.nothing, 0f, "This is A dummy") }));
            Skill_db.Add("dummy B", new Skill("Test Skill", "Does nothing", p_type.enemy, new List<BattleEffect>() { new BattleEffect(e_type.damage, 5f, "This is B dummy") }));
            Skill_db.Add("smarty A", new Skill("Test Skill", "Does nothing", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.nothing, 0f, "This is A smarty") }));
            Skill_db.Add("smarty B", new Skill("Test Skill", "Does nothing", p_type.player, new List<BattleEffect>() { new BattleEffect(e_type.damage, 5f, "This is B smarty") }));

            Participant_db.Add("person A", new PlayerScriptableObject("Adam"));
            Participant_db["person A"].AddSkill(Skill_db["smarty B"]);
            Participant_db.Add("person B", new PlayerScriptableObject("Ben"));
            Participant_db["person B"].AddSkill(Skill_db["smarty A"]);

            Participant_db.Add("enemy A", new MonsterScriptableObject("Azazel"));
            Participant_db["enemy A"].AddSkill(Skill_db["dummy A"]);
            Participant_db.Add("enemy B", new MonsterScriptableObject("Beelzebub"));
            Participant_db["enemy B"].AddSkill(Skill_db["dummy B"]);
            environments = new List<Environment>();
            
            environments.Add(new Environment("desert",environmentPrefabs[0]));
            environments.Add(new Environment("swamp",environmentPrefabs[1]));
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


