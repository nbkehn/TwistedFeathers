using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    ArrayList battle_participants;
    int currentTurn;
    bool waiting;

    void queueSkill(Skill skill, Participant user, Participant target)
    {
        BattleEffect effect = skill.Effect;
        effect.User = user;
        effect.Target = target;
        effect.Turnstamp = currentTurn;
        pq.Add(effect);
    }

    void resolveEffects()
    {
        while (pq.Count != 0 && pq.Min.Turnstamp <= currentTurn)
        {
            Debug.Log(pq.Min.Message);
            pq.Remove(pq.Min);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = 0;

        pq = new SortedSet<BattleEffect>(new EffectComparator());
        battle_participants = new ArrayList();
        

        //pq.Add(GameManager.Skill_db["dummy A"].Effect);

        //pq.Add(GameManager.Skill_db["dummy B"].Effect);

        //Debug.Log(pq.Min.Message);

        waiting = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (waiting)
        {
            if (Input.GetButtonDown("Add Effect"))
            {
                pq.Add(GameManager.Skill_db["dummy A"].Effect);
            }
            if (Input.GetButtonDown("Turn Pass"))
            {
                waiting = false;
                pq.Add(GameManager.Skill_db["dummy B"].Effect);
            }
        }
        else
        {
            resolveEffects();
            currentTurn++;
            waiting = true;

        }
        
    }
}
