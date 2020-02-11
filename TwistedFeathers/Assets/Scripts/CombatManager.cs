using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;
    ArrayList battle_participants;
    int currentTurn; 

    void queueSkill(Skill skill, Participant user, Participant target)
    {
        BattleEffect effect = skill.Effect;
        effect.User = user;
        effect.Target = target;
        pq.Add(effect);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTurn = 0;

        pq = new SortedSet<BattleEffect>(new EffectComparator());
        battle_participants = new ArrayList();
        

        pq.Add(GameManager.Skill_db["dummy A"].Effect);

        pq.Add(GameManager.Skill_db["dummy B"].Effect);

        Debug.Log(pq.Min.Message);

        do
        {
            currentTurn++;

        }
        while (false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
