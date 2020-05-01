using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{

    public GameObject creditsObjects;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject.FindGameObjectWithTag("Music2").GetComponent<Music>().StopMusic();
        GameObject.FindGameObjectWithTag("Music1").GetComponent<Music>().PlayMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void hideTitle(){
        creditsObjects.SetActive(true);
    }

    public void showTitle(){
        creditsObjects.SetActive(false);
    }
}
