using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwistedFeathers
{
    public class Environment : Participant
    {
        public GameObject miniIcon;
        public int envListIndex;
        private List<KeyValuePair<int, Skill>> skills;

        public Environment() : base(p_type.environment, "")
        {

        }

        public Environment(string name, GameObject Icon) : base(p_type.environment, name)
        {
            this.skills = new List<KeyValuePair<int, Skill>>();
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

        public Environment(string name, GameObject Icon, List<KeyValuePair<int, Skill>> skills) : base(p_type.environment, name)
        {
            this.skills = skills;
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

        public new List<KeyValuePair<int, Skill>> Skills { get => skills; set => skills = value; }
    }
}
