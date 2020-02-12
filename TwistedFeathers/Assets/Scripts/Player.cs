using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Participant
{
    public Player() : base(p_type.player, "")
    {

    }

    public Player(string name) : base(p_type.player, name)
    {

    }

}


