using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScriptableObject : Participant
{
    public MonsterScriptableObject () : base(p_type.enemy, "")
    {
        
    }

    public MonsterScriptableObject(string name) : base(p_type.enemy, name)
    {

    }

}
