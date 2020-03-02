using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BattleParticipant
{
    public Monster () : base()
    {

    }

    public Monster(string name) : base(p_type.enemy, name)
    {

    }
}
