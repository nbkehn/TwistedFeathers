using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Participant
{
    public Monster () : base(p_type.enemy, "")
    {

    }

    public Monster(string name) : base(p_type.enemy, name)
    {

    }
}
