﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwistedFeathers
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance { get { return _instance; } }
        //Index in enum

        public Player player1 = new Player();
        public Player player2 = new Player();


        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } else {
                _instance = this;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void selectCharacter(int playerType){
            if (playerType == 1){
                player1.setPlayerClass(s_type.Fighter);
                player2.setPlayerClass(s_type.Rogue);
            } else {
                player1.setPlayerClass(s_type.Rogue);
                player2.setPlayerClass(s_type.Fighter);
            }
            Debug.Log(player1.getPlayerClass());
        }

        public void next(){
            if(player1.getPlayerClass() != s_type.None){
                SceneManager.LoadScene("TestScene");
            }
        }

        public void awardEXP(int exp){
            player1.totalEXP += exp;
            player2.totalEXP += exp;
        }
    }
}
