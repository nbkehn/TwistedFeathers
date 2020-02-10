using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    SortedSet<BattleEffect> pq;

    // Start is called before the first frame update
    void Start()
    {

        pq = new SortedSet<BattleEffect>(new EffectComparator());
        

        pq.Add(GameManager.Skill_db["dummy A"].Effect);

        pq.Add(GameManager.Skill_db["dummy B"].Effect);

        Debug.Log(pq.Min.Message);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
