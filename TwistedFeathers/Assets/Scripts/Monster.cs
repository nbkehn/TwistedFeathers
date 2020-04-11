using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public class Monster : BattleParticipant
    {
        public int EnemyType;
        public Monster() : base()
        {

        }

        public Monster(string name) : base(p_type.enemy, name)
        {
            
        }
        //add & set attack list (primary set to 0, and so on)
        //add & set utility list (primary set to 0, and so on)
        //set these skills on enemy creation

        public Monster(string name, int EnemyType) : base(p_type.enemy, name)
        {
            this.EnemyType = EnemyType;
        }

        public int getEnemyType()
        {
            return EnemyType;
        }
        //add getter for attack and utility list
    }
}
