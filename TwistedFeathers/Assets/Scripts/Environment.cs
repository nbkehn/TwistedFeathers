using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : Participant 
{
    public Environment() : base(p_type.environment, "")
    {

    }

    public Environment(string name) : base(p_type.environment, name)
    {

    }

}
