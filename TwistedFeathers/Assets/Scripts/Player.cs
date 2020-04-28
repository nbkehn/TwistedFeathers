using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{



    public class Player : BattleParticipant
    {
        private static s_type playerClass;
        public int totalEXP;

        public bool isDead;
        
        public s_type getPlayerClass(){
            return playerClass;
        }
         public void setPlayerClass(s_type value){
            playerClass = value;
        }

        public Player() : base()
        {
            playerClass = s_type.None;
            totalEXP = 0;
            isDead = false;
        }

        public Player(s_type name) : base(p_type.player, name)
        {
            playerClass = name;
            totalEXP = 0;
            isDead = false;
        }


    }
}


