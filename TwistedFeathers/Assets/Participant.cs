using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum p_type {none, player, enemy, environment, weather};

public abstract class Participant
{
    private p_type type;

    private Skill[] skills;
}


public class Participant_B : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
