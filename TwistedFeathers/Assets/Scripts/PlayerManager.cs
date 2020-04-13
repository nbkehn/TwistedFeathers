using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }
    //Index in enum
    private static int player1 = -1;
    public static int Player1 { get { return player1; } }

    private static int player2 = -1;
    public static int Player2 { get { return player2; } }

    public enum player_type
    {
        rogue,
        fighter
    };

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
            player1 = 1;
            player2 = 0;
        } else {
            player1 = 0;
            player2 = 1;
        }
    }

    public void next(){
        if(player1 != -1){
            SceneManager.LoadScene("TestScene");
        }
    }
}
