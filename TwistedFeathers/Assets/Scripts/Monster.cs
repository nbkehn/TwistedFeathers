using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public class Monster : BattleParticipant
    {

        public bool isDead;
        public int EnemyType;

        public Monster() : base()
        {
            isDead = false;
        }

        public Monster(s_type name) : base(p_type.enemy, name)
        {
            isDead = false;
        }
        //add & set attack list (primary set to 0, and so on)
        //add & set utility list (primary set to 0, and so on)
        //set these skills on enemy creation

        public Monster(s_type name, int EnemyType) : base(p_type.enemy, name)
        {
            isDead = false;
            this.EnemyType = EnemyType;
        }

        public int getEnemyType()
        {
            return EnemyType;
        }
        //add getter for attack and utility list
    }
}
