using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    ArrayList battle_participants;
    int currentTurn;
    bool waitingPlayer;
    int protagonistIndex;
    public GameObject ForecastOpener;
    static string forecastText;
    public GameObject textPrefab;

    private GameObject newText;
    public GameObject forecastContent;
    int numTexts = 0;

    public static string ForecastText { get => forecastText; set => forecastText = value; }

    //Method for taking a skill and queueing the effect into the PQ
    void queueSkill(Skill skill, Participant user, Participant target)
    {
        BattleEffect effect = skill.Effect;
        effect.User = user;
        effect.Target = target;
        effect.Turnstamp = currentTurn;
        pq.Add(effect);
    }

    //Method for resolving all effects set to happen this turn in PQ
    void resolveEffects()
    {
        Debug.Log("Effects Resolving!");
        while (pq.Count != 0 && pq.Min.Turnstamp <= currentTurn)
        {
            Debug.Log(pq.Min.Message);
            pq.Remove(pq.Min);
        }
        Debug.Log("Effects Resolved!");
    }

    // Start is called before the first frame update
    void Start()
    {

        ForecastText = "";
        currentTurn = 0;
        pq = new SortedSet<BattleEffect>(new EffectComparator());
        battle_participants = new ArrayList();
        protagonistIndex = 0; 
        //Dummy values for testing purposes
        battle_participants.Add(GameManager.Participant_db["person A"]);
        battle_participants.Add(GameManager.Participant_db["enemy B"]);

        waitingPlayer = false;
        Debug.Log("TURN BEGIN");
    }
    
    public void SelectSkill(string skill){
        Debug.Log(skill); // change this to actually do what the skill does
        chooseSkill();
    }

    public void chooseSkill(){
        Player protag = (Player) battle_participants[protagonistIndex]; 
        queueSkill((Skill) protag.Skills[Random.Range(0, protag.Skills.Count)], protag, null);
        waitingPlayer = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingPlayer)
        {
            //Effects are resolved and turn ends
            resolveEffects();
            Debug.Log("TURN END");
            //New turn beings here
            Debug.Log("TURN BEGIN");
            currentTurn++;
            foreach (Participant part in battle_participants)
            {
                if (part.Type != p_type.player)
                {
                    queueSkill((Skill) part.Skills[Random.Range(0, part.Skills.Count)], part, null);
                }
            }

            //Forecast

            Debug.Log("Forecast Begins!");
            
            foreach (BattleEffect eff in pq)
            {
                newText = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                newText.transform.SetParent(forecastContent.transform, false);
                newText.GetComponent<RectTransform>().localScale = new Vector3(0.6968032f, 1.7355f, 1.7355f);
                newText.transform.position = new Vector3(forecastContent.transform.position.x + 275, forecastContent.transform.position.y - 40*numTexts -30);
                newText.GetComponent<Text>().text = eff.User.Name;
                newText.GetComponent<Text>().color = Color.white;

                numTexts++;
                Debug.Log(eff.User.Name);
            }
            ForecastOpener.GetComponent<ButtonHandler>().newForecast();
            Debug.Log("Forecast Over!");

            waitingPlayer = true;

        }
        
    }
}
