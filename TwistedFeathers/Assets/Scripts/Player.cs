using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public enum p_class
    {
        rogue,
        fighter,
        notAssigned
    };    



    public class Player : BattleParticipant
    {
        private static p_class playerClass;
        public int totalEXP;

        public bool isDead;
        
        public p_class getPlayerClass(){
            return playerClass;
        }
         public void setPlayerClass(p_class value){
            playerClass = value;
        }

        public Player() : base()
        {
            playerClass = p_class.notAssigned;
            totalEXP = 0;
            isDead = false;
        }

        public Player(s_type name) : base(p_type.player, name)
        {
            playerClass = p_class.notAssigned;
            totalEXP = 0;
            isDead = false;
        }


    }
}


