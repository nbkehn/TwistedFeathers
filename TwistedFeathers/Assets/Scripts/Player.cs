﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{

    public class Player : BattleParticipant
    {
        public Player() : base()
        {

        }

        public Player(string name) : base(p_type.player, name)
        {

        }


    }
}

