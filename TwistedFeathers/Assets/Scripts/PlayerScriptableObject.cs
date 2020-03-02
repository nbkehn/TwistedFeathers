using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptableObject : Participant
{

    public PlayerScriptableObject() : base(p_type.player, "")
    {

    }

    public PlayerScriptableObject(string name) : base(p_type.player, name)
    {

    }
    
}
