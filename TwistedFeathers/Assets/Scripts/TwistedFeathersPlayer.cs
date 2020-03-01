using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// had to change the name of the class Player due to issues where
// in photon pun there already exists a Player class
public class TwistedFeathersPlayer : Participant
{
    public TwistedFeathersPlayer() : base(p_type.player, "")
    {

    }

    public TwistedFeathersPlayer(string name) : base(p_type.player, name)
    {

    }

}


