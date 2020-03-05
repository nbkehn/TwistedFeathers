using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public class Environment : Participant
    {
        public GameObject miniIcon;
        public int envListIndex;

        public Environment() : base(p_type.environment, "")
        {

        }

        public Environment(string name, GameObject Icon) : base(p_type.environment, name)
        {
            miniIcon = Icon;
            Debug.Log(miniIcon);
            if (name == "swamp")
            {
                envListIndex = 1;
            }

            if (name == "desert")
            {
                envListIndex = 0;
            }
        }

        public Environment(string name, GameObject Icon, List<Skill> skills) : base(p_type.environment, name, skills)
        {
            miniIcon = Icon;
            Debug.Log(miniIcon);
            if (name == "swamp")
            {
                envListIndex = 1;
            }

            if (name == "desert")
            {
                envListIndex = 0;
            }
        }

    }
}
