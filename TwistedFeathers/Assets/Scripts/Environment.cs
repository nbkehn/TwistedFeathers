﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : Participant 
{
    public GameObject miniIcon;
    public Environment() : base(p_type.environment, "")
    {

    }

    public Environment(string name, GameObject Icon) : base(p_type.environment, name)
    {
        miniIcon = Icon;
        Debug.Log(miniIcon);
    }

}
