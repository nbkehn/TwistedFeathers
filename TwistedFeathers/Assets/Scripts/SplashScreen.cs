using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject.FindGameObjectWithTag("Music1").GetComponent<Music>().PlayMusic();
        //GameObject.FindGameObjectWithTag("Music2").GetComponent<Music>().StopMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
