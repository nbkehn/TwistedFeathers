using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    List<Participant> battle_participants;
    int currentTurn;
    bool waitingPlayer;
    int protagonistIndex;
    public GameObject ForecastOpener;
    static string forecastText;

    public static string ForecastText { get => forecastText; set => forecastText = value; }

    //Method for taking a skill and queueing the effect into the PQ
    void queueSkill(Skill skill, Participant user, Participant target)
    {
        foreach(BattleEffect effect in skill.Effect)
        {
            BattleEffect battle_effect = effect;
            battle_effect.select(user, target, currentTurn);
            pq.Add(battle_effect);
        }
    }

    //Method for resolving all effects set to happen this turn in PQ
    void resolveEffects()
    {
        Debug.Log("Effects Resolving!");
        while (pq.Count != 0 && pq.Min.Turnstamp <= currentTurn)
        {
            pq.Min.run();
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
        battle_participants = new List<Participant>();
        protagonistIndex = 0; 
        //Dummy values for testing purposes
        battle_participants.Add(GameManager.Participant_db["person A"]);
        battle_participants.Add(GameManager.Participant_db["enemy B"]);

        waitingPlayer = false;
        Debug.Log("TURN BEGIN");
    }

    public void chooseSkill(){
        Player protag = (Player) battle_participants[protagonistIndex]; 
        queueSkill( protag.Skills[Random.Range(0, protag.Skills.Count)], protag, null);
        waitingPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!waitingPlayer)
        {
            //Effects are resolved and turn ends
            resolveEffects();
            //Testing HP damage
            Debug.Log("Adam HP: " + battle_participants[protagonistIndex].Current_hp);
            Debug.Log("TURN END");
            //New turn beings here
            Debug.Log("TURN BEGIN");
            currentTurn++;
            foreach (Participant part in battle_participants)
            {
                if (part.Type != p_type.player)
                {
                    queueSkill((Skill) part.Skills[Random.Range(0, part.Skills.Count)], part, battle_participants[protagonistIndex]);
                }
            }

            //Forecast

            Debug.Log("Forecast Begins!");
            foreach (BattleEffect eff in pq)
            {
                ForecastText += eff.User.Name;
                ForecastText += "\n";
                Debug.Log(eff.User.Name);
            }
            ForecastOpener.GetComponent<ButtonHandler>().newForecast();
            Debug.Log("Forecast Over!");

            waitingPlayer = true;

        }
        
    }
}
