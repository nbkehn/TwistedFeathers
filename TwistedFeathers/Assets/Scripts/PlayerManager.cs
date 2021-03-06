﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TwistedFeathers
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance { get { return _instance; } }
        //Index in enum

        public Player player1 = new Player();
        public Player player2 = new Player();
        public GameObject gameManagerPrefab;


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
            Debug.Log("Begin Character selection");
            if (playerType == 0){
                player1.setPlayerClass(s_type.Rogue);
                Debug.Log("Case 1: Player 1 is a: " + player1.getPlayerClass());
                player2.setPlayerClass(s_type.Fighter);
                Debug.Log("Case 1: Player 2 is a: " + player2.getPlayerClass());
            } else {
                player1.setPlayerClass(s_type.Fighter);
                Debug.Log("Case 2: Player 1 is a: " + player1.getPlayerClass());
                player2.setPlayerClass(s_type.Rogue);
                Debug.Log("Case 2: Player 2 is a: " + player2.getPlayerClass());
            }
            Debug.Log("Player 1 is a: " + player1.getPlayerClass());
            Debug.Log("Player 2 is a: " + player2.getPlayerClass());
            Debug.Log("End Character selection");
        }

        public void next(){
            if(player1.getPlayerClass() != s_type.None){
                SceneManager.LoadScene("TestScene");
                this.StartCoroutine("loadManager");
            }
        }

        public IEnumerator loadManager(){
            int count = 0;
            while(!(SceneManager.GetActiveScene ().name == "TestScene") && count < 10){
                //WAIT
                yield return new WaitForSeconds(.1f);
            }
            GameObject gameManager = GameObject.Instantiate(this.gameManagerPrefab, new Vector3(0,0,0), Quaternion.identity);
            gameManager.name = "GameManager";

        }

        public void awardEXP(int exp){
            player1.totalEXP += exp;
            player2.totalEXP += exp;
        }
    }
}
