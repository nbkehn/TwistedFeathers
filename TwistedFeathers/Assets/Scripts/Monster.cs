using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public class Monster : BattleParticipant
    {

        public bool isDead;
        public Monster() : base()
        {
            isDead = false;
        }

        public Monster(s_type name) : base(p_type.enemy, name)
        {
            isDead = false;
        }
    }
}
